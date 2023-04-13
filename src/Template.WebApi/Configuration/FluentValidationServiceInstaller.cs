using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Template.WebApi.Configuration;

public class FluentValidationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
    }
}
