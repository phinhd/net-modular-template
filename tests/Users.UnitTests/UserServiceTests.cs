using BuildingBlocks.Application.Exceptions;
using Moq;
using Users.Application.Services;
using Users.Domain.Entities;
using Users.Domain.Interfaces;

namespace Users.UnitTests;

public sealed class UserServiceTests
{
    private readonly Mock<IUsersUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_users.Object);
        _sut = new UserService(_unitOfWork.Object);
    }

    [Fact]
    public async Task GetById_ReturnsDto_WhenUserExists()
    {
        var user = User.Create("user@example.com", "hash", "Jane Doe");
        _users.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var dto = await _sut.GetByIdAsync(user.Id);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal("Jane Doe", dto.FullName);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public async Task GetById_ThrowsNotFound_WhenUserDoesNotExist()
    {
        _users.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(Guid.NewGuid()));
    }
}
