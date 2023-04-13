using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Template.Application.Common;
using Template.Application.Common.Interfaces;
using Template.Application.Common.Services;
using Template.Application.Settings;

namespace Template.WebApi.Configuration;

[ExcludeFromCodeCoverage]
public class PollyPoliciesServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PollyPoliciesSettings>(configuration?.GetSection(nameof(PollyPoliciesSettings)));
        services.AddSingleton<IPollyPolicies, PollyPolicies>();
        services.AddScoped<ICustomServiceProvider, ServiceProviderWrapper>();
    }
}
