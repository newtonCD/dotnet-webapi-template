using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using Template.Application.Common.Interfaces;
using Template.Application.Settings;
using Template.Infrastructure.Caching;

namespace Template.WebApi.Configuration;

[ExcludeFromCodeCoverage]
public class CacheServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheSettings>(configuration?.GetSection(nameof(CacheSettings)));
        services.AddScoped<IDistributedCacheWrapper, DistributedCacheWrapper>();
        services.AddScoped<ICacheService, CacheService>();

        var cacheSettings = configuration.GetSection(nameof(CacheSettings)).Get<CacheSettings>();
        switch (cacheSettings.Type)
        {
            case "Memory":
                services.AddDistributedMemoryCache();
                break;
            case "Redis":
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheSettings.RedisConnectionString;
                });
                break;
            default:
                throw new NotSupportedException($"O tipo de cache '{cacheSettings.Type}' não é suportado.");
        }
    }
}
