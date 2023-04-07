using Application.Common.Interfaces;
using Application.Settings;
using Microsoft.Extensions.Options;
using System;

namespace Application.Common.Cache;

public abstract class CacheableQueryBase
{
    public abstract string CacheKey { get; }
    public IOptions<CacheSettings> CacheSettings { get; set; }

    public TimeSpan? GetCacheExpiration(ICustomServiceProvider serviceProvider)
    {
        CacheSettings ??= serviceProvider?.GetService<IOptions<CacheSettings>>();

        if (CacheSettings?.Value == null)
        {
            return null;
        }

        return TimeSpan.FromMinutes(CacheSettings.Value.ExpirationInMinutes);
    }
}
