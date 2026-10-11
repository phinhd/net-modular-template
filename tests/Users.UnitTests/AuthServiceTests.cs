using BuildingBlocks.Application.Exceptions;
using Moq;
using Users.Application.DTOs;
using Users.Application.Interfaces;
using Users.Application.Services;
using Users.Domain.Entities;
using Users.Domain.Interfaces;

namespace Users.UnitTests;

public sealed class AuthServiceTests
{
    private const string Password = "Password123!";

    private readonly Mock<IUsersUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();
    private readonly Mock<IEmailSender> _emailSender = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_users.Object);
        _jwtTokenService.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("jwt-token");
        _sut = new AuthService(_unitOfWork.Object, _jwtTokenService.Object, _emailSender.Object);
    }

    [Fact]
    public async Task Register_PersistsUser_SendsWelcomeEmail_AndReturnsToken()
    {
        var result = await _sut.RegisterAsync(new RegisterRequest("  New.User@Example.com ", Password, "New User"));

        Assert.Equal("new.user@example.com", result.Email);
        Assert.Equal("jwt-token", result.Token);
        Assert.Equal(User.DefaultRole, result.Role);
        _users.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == "new.user@example.com"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.Verify(e => e.SendWelcomeEmailAsync("new.user@example.com", "New User", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_HashesPassword_NeverStoresPlainText()
    {
        User? saved = null;
        _users.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => saved = u);

        await _sut.RegisterAsync(new RegisterRequest("user@example.com", Password, "User"));

        Assert.NotNull(saved);
        Assert.NotEqual(Password, saved.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(Password, saved.PasswordHash));
    }

    [Fact]
    public async Task Register_ThrowsConflict_WhenEmailAlreadyExists()
    {
        _users.Setup(r => r.EmailExistsAsync("taken@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(
            () => _sut.RegisterAsync(new RegisterRequest("Taken@Example.com", Password, "Test User")));

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _emailSender.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenCredentialsAreValid()
    {
        var user = CreateUser("user@example.com", Password);
        _users.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _sut.LoginAsync(new LoginRequest("USER@example.com", Password));

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("jwt-token", result.Token);
    }

    [Fact]
    public async Task Login_Throws_WhenUserDoesNotExist()
    {
        _users.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => _sut.LoginAsync(new LoginRequest("ghost@example.com", "whatever")));
    }

    [Fact]
    public async Task Login_Throws_WhenPasswordIsWrong()
    {
        var user = CreateUser("user@example.com", Password);
        _users.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => _sut.LoginAsync(new LoginRequest("user@example.com", "WrongPassword!")));
        _jwtTokenService.Verify(t => t.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Login_Throws_WhenUserIsDeactivated()
    {
        var user = CreateUser("user@example.com", Password);
        user.Deactivate();
        _users.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => _sut.LoginAsync(new LoginRequest("user@example.com", Password)));
    }

    private static User CreateUser(string email, string password)
        => User.Create(email, BCrypt.Net.BCrypt.HashPassword(password, workFactor: 4), "Test User");
}
