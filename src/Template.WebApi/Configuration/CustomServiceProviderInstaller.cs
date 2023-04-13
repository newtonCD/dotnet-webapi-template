using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Common.Interfaces;
using Template.Application.Common.Services;

namespace Template.WebApi.Configuration;

public class CustomServiceProviderInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICustomServiceProvider, ServiceProviderWrapper>();
    }
}