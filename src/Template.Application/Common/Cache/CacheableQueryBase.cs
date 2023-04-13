using Microsoft.Extensions.Options;
using System;
using Template.Application.Common.Interfaces;
using Template.Application.Settings;

namespace Template.Application.Common.Cache;

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
