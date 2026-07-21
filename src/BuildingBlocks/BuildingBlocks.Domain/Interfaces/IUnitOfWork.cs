namespace BuildingBlocks.Domain.Interfaces;

/// <summary>
/// Each module defines its own IUnitOfWork (e.g. Users.Domain.Interfaces.IUsersUnitOfWork)
/// that extends this, exposing that module's repositories as properties.
/// Implemented in {Module}.Infrastructure, backed by that module's DbContext.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
