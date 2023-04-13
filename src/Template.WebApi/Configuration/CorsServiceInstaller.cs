using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Template.WebApi.Configuration;

[ExcludeFromCodeCoverage]
public class CorsServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
                         options.AddPolicy("CorsPolicy", cpb =>
                            cpb
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod()));
    }
}