using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Caching;

public class DistributedCacheWrapper : IDistributedCacheWrapper
{
    private readonly IDistributedCache _cache;

    public DistributedCacheWrapper(IDistributedCache cache)
    {
        _cache = cache;
    }

    public Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.GetStringAsync(key, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key, cancellationToken);
    }

    public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
    {
        return _cache.SetStringAsync(key, value, options, cancellationToken);
    }
}