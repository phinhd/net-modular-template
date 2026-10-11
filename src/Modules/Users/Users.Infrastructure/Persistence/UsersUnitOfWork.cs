using Users.Domain.Interfaces;

namespace Users.Infrastructure.Persistence;

public sealed class UsersUnitOfWork : IUsersUnitOfWork
{
    private readonly UsersDbContext _context;
    private IUserRepository? _users;

    public UsersUnitOfWork(UsersDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
