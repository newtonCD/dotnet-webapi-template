using System;

namespace Template.Application.Common.Interfaces;

public interface ICacheable
{
    string CacheKey { get; }
    TimeSpan? GetCacheExpiration(ICustomServiceProvider serviceProvider);
}