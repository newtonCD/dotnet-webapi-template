using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Template.WebApi.Configuration;

[ExcludeFromCodeCoverage]
public class FluentValidationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
    }
}
