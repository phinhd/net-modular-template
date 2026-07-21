using System.Linq.Expressions;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Generic EF Core repository base. Each module's concrete repository
/// (e.g. UserRepository : RepositoryBase&lt;User&gt;, IUserRepository) inherits this
/// and only adds module-specific queries on top.
/// </summary>
public abstract class RepositoryBase<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DbContext Context;
    protected readonly DbSet<T> DbSet;

    protected RepositoryBase(DbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

    public virtual async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet.Where(e => !e.IsDeleted);
        if (predicate is not null)
            query = query.Where(predicate);

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> ListPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet.Where(e => !e.IsDeleted);
        if (predicate is not null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public virtual void Update(T entity) => DbSet.Update(entity);

    public virtual void Remove(T entity) => DbSet.Remove(entity);
}
