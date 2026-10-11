using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Interfaces;

namespace Users.Infrastructure.Persistence;

public sealed class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = User.NormalizeEmail(email);
        return await DbSet.FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = User.NormalizeEmail(email);
        return await DbSet.AnyAsync(u => u.Email == normalized, cancellationToken);
    }
}
