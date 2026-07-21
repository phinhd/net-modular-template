using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Interfaces;

namespace Users.Infrastructure.Persistence;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(UsersDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(
            u => u.Email == email.Trim().ToLower() && !u.IsDeleted,
            cancellationToken);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(
            u => u.Email == email.Trim().ToLower() && !u.IsDeleted,
            cancellationToken);
}
