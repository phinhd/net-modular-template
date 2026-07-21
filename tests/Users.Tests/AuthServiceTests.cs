using BuildingBlocks.Application.Exceptions;
using Moq;
using Users.Application.DTOs;
using Users.Application.Services;
using Users.Domain.Entities;
using Users.Domain.Interfaces;
using Xunit;

namespace Users.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUsersUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_userRepository.Object);
        _sut = new AuthService(_unitOfWork.Object, _jwtTokenService.Object);
    }

    [Fact]
    public async Task Register_ThrowsBusinessRuleException_WhenEmailAlreadyExists()
    {
        _userRepository.Setup(r => r.EmailExistsAsync("taken@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new RegisterRequest("taken@example.com", "Password123!", "Test User");

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.RegisterAsync(request));
    }

    [Fact]
    public async Task Login_ThrowsAuthenticationFailedException_WhenUserDoesNotExist()
    {
        _userRepository.Setup(r => r.GetByEmailAsync("ghost@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var request = new LoginRequest("ghost@example.com", "whatever");

        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _sut.LoginAsync(request));
    }
}
