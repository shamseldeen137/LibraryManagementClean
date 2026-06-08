using LibraryManagement.Domain.Common;

namespace LibraryManagement.Application.Common;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
