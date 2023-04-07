#nullable enable
using Application.Common;
using Application.Common.Interfaces;
using Application.Settings;
using Domain.Interfaces;
using Infrastructure.Caching;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string commandConnectionString = "CommandInMemoryDb";
        string queryConnectionString = "QueryInMemoryDb";

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<AppCommandDbContext>(options =>
                 options.UseInMemoryDatabase(commandConnectionString));

            services.AddDbContext<AppQueryDbContext>(options =>
                options.UseInMemoryDatabase(queryConnectionString));
        }
        else
        {
            commandConnectionString = configuration.GetConnectionString("CommandConnectionString");
            queryConnectionString = configuration.GetConnectionString("QueryConnectionString");

            services.AddDbContext<AppCommandDbContext>(options =>
                options.UseSqlServer(commandConnectionString));

            services.AddDbContext<AppQueryDbContext>(options =>
                options.UseSqlServer(queryConnectionString));
        }

        services.AddScoped<IDbOperationConfiguration>(_ =>
                                                        new DbOperationConfiguration(
                                                            commandConnectionString,
                                                            queryConnectionString));

        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAppCommandDbContext, AppCommandDbContext>();
        services.AddScoped<IAppQueryDbContext, AppQueryDbContext>();

        // Registra a interface genérica do repositório e sua implementação.
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        #region Registra automaticamente todas as interfaces de repositório e suas implementações correspondentes.

        Assembly? domainAssembly = Assembly.GetAssembly(typeof(IBaseRepository<>));
        Assembly? infraAssembly = Assembly.GetAssembly(typeof(BaseRepository<>));

        if (domainAssembly == null || infraAssembly == null)
        {
            throw new InvalidOperationException("Não foi possível encontrar os assemblies de domínio e/ou infraestrutura.");
        }

        List<Type>? repositoryTypes = domainAssembly.GetTypes()
            .Where(t => t.IsInterface && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseRepository<>)))
            .ToList();

        foreach (Type repositoryType in repositoryTypes)
        {
            Type? implementationType = Array.Find(infraAssembly.GetTypes(), t => t.IsClass && t.GetInterfaces().Any(i => i == repositoryType));

            if (implementationType != null)
            {
                services.AddScoped(repositoryType, implementationType);
            }
            else
            {
                throw new InvalidOperationException($"Não foi possível encontrar a implementação correspondente para a interface '{repositoryType.FullName}'.");
            }
        }

        #endregion

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
