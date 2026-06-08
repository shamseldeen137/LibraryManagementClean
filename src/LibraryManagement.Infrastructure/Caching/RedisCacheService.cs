using System.Text.Json;
using LibraryManagement.Application.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace LibraryManagement.Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        return value is null ? default : JsonSerializer.Deserialize<T>(value, JsonOptions);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        return _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(value, JsonOptions),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration },
            cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key, cancellationToken);
    }
}
