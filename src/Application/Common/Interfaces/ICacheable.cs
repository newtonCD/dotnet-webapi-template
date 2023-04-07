using System;

namespace Application.Common.Interfaces;

public interface ICacheable
{
    string CacheKey { get; }
    TimeSpan? GetCacheExpiration(ICustomServiceProvider serviceProvider);
}