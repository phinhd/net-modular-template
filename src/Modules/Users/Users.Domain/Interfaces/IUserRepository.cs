using BuildingBlocks.Domain.Interfaces;
using Users.Domain.Entities;

namespace Users.Domain.Interfaces;

/// <summary>
/// Extends the generic repository with module-specific queries.
/// Implemented in Users.Infrastructure/Persistence/UserRepository.cs.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
