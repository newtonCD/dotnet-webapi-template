using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.WebApi.Presenters.Swagger;

namespace Template.WebApi.Configuration;

public class SwaggerServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
        services.ConfigureOptions<ConfigureSwaggerOptions>();
    }
}
