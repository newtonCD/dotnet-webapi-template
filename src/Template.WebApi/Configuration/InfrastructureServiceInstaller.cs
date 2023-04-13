#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Template.Application.Common;
using Template.Application.Common.Interfaces;
using Template.Infrastructure.Persistance;

namespace Template.WebApi.Configuration;

public class InfrastructureServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services
             .Scan(
                 selector => selector
                     .FromAssemblies(
                         Infrastructure.AssemblyEntryPoint.Assembly)
                     .AddClasses(false)
                     .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                     .AsMatchingInterface()
                     .WithScopedLifetime());

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
    }
}
