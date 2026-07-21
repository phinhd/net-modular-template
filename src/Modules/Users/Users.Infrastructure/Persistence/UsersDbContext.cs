using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;

namespace Users.Infrastructure.Persistence;

/// <summary>
/// Each module owns its own DbContext (own migrations history, own schema).
/// If you need cross-module data, query through the other module's public
/// Application service/API — never reach into another module's DbContext directly.
/// </summary>
public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("users");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
