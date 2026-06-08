using LibraryManagement.Application.Common;
using LibraryManagement.Domain.Common;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Mongo;

public sealed class MongoRepository<T> : IRepository<T> where T : Entity
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(MongoDbContext context)
    {
        _collection = context.Collection<T>();
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(FilterDefinition<T>.Empty).ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var total = await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        var items = await _collection.Find(FilterDefinition<T>.Empty)
            .SortByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, total, page, pageSize);
    }

    public async Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);
        return entity;
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
    }
}
