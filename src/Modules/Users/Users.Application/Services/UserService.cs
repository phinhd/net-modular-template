using BuildingBlocks.Application.Exceptions;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Interfaces;

namespace Users.Application.Services;

/// <summary>
/// Read-side use cases for the Users module. Other modules should call this
/// (or an API endpoint built on it) instead of querying the users schema directly.
/// </summary>
public sealed class UserService
{
    private readonly IUsersUnitOfWork _unitOfWork;

    public UserService(IUsersUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), id);

        return new UserDto(user.Id, user.Email, user.FullName, user.Role, user.IsActive, user.CreatedAtUtc);
    }
}
