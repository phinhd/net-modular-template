using System.Linq.Expressions;
using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Interfaces;

/// <summary>
/// Generic repository contract. Business-facing interfaces (e.g. IUserRepository)
/// live in {Module}.Domain/Interfaces and extend this; the implementation lives in
/// {Module}.Infrastructure/Persistence.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<T> Items, int TotalCount)> ListPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}
