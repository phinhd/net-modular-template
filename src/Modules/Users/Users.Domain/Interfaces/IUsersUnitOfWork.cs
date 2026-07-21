using BuildingBlocks.Domain.Interfaces;

namespace Users.Domain.Interfaces;

/// <summary>
/// Module-scoped Unit of Work. Every module gets its own like this one,
/// exposing that module's repositories. Implemented in Users.Infrastructure.
/// </summary>
public interface IUsersUnitOfWork : IUnitOfWork
{
    IUserRepository Users { get; }
}
