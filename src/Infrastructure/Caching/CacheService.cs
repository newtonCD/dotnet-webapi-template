using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IDistributedCacheWrapper _cache;

    public CacheService(IDistributedCacheWrapper cache)
    {
        _cache = cache;
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        string cachedValueJson = await _cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
        return cachedValueJson is null ? default : JsonSerializer.Deserialize<T>(cachedValueJson);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiresIn = null, CancellationToken cancellationToken = default)
    {
        var cacheEntryOptions = new DistributedCacheEntryOptions();
        if (expiresIn.HasValue)
        {
            cacheEntryOptions.SetAbsoluteExpiration(expiresIn.Value);
        }

        string valueJson = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, valueJson, cacheEntryOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
    }
}