using Application.Common;
using Application.Common.Behaviors;
using Application.Common.Interfaces;
using Application.Common.Services;
using Application.Settings;
using AspNetCoreRateLimit;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Infrastructure.Caching;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;
using WebApi.Filters;
using WebApi.Presenters.Swagger;

namespace Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ConfigureServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.ConfigureLogginBehavior(configuration);
        services.ConfigurePollyPolicies(configuration);
        services.ConfigureCustomServiceProvider();
        services.ConfigureCors();
        services.ConfigureControllers();
        services.ConfigureFluentValidation();
        services.ConfigureProblemDetails();
        services.ConfigureMediatRAndBehaviors();
        services.ConfigureCacheServices(configuration);
        services.AddInfrastructure(configuration);
        services.ConfigureApiVersioning();
        services.ConfigureSwagger();
        services.ConfigureRateLimiting(configuration);
        services.ConfigureAuthentication();
        services.ConfigureHealthChecksAndRouting();

        return services;
    }

    private static void ConfigurePollyPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PollyPoliciesSettings>(configuration?.GetSection("PollyPoliciesSettings"));
        services.AddSingleton<IPollyPolicies, PollyPolicies>();
        services.AddScoped<ICustomServiceProvider, ServiceProviderWrapper>();
    }

    private static void ConfigureCustomServiceProvider(this IServiceCollection services)
    {
        services.AddScoped<ICustomServiceProvider, ServiceProviderWrapper>();
    }

    private static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(co =>
                         co.AddPolicy("CorsPolicy", cpb =>
                            cpb
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod()));
    }

    private static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            options.Filters.Add(typeof(ApiExceptionHandlingFilterAttribute));
        })
        .AddNewtonsoftJson(
            options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
        .AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    private static void ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
    }

    private static void ConfigureProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.Rethrow<NotSupportedException>();
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
            options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
        })
        .AddProblemDetailsConventions();
    }

    private static void ConfigureMediatRAndBehaviors(this IServiceCollection services)
    {
        Assembly applicationAssembly = typeof(Application.AssemblyEntryPoint).Assembly;

        services.AddValidatorsFromAssembly(applicationAssembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PollyRetryBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
    }

    private static void ConfigureCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
        services.AddScoped<IDistributedCacheWrapper, DistributedCacheWrapper>();
        services.AddScoped<ICacheService, CacheService>();

        CacheSettings cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>();
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

    private static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(option =>
        {
            option.DefaultApiVersion = new ApiVersion(1, 0);
            option.AssumeDefaultVersionWhenUnspecified = true;
            option.ReportApiVersions = true;
        });
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    private static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
        services.ConfigureOptions<ConfigureSwaggerOptions>();
    }

    private static void ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddInMemoryRateLimiting();
    }

    private static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        services.AddAuthorization();
    }

    private static void ConfigureHealthChecksAndRouting(this IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddRouting(options => options.LowercaseUrls = true);
    }

    private static void ConfigureLogginBehavior(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoggingSettings>(configuration.GetSection("LoggingSettings"));
    }
}