using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Settings;

namespace Template.WebApi.Configuration;

public class LogginSettingsServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoggingSettings>(configuration?.GetSection(nameof(LoggingSettings)));
    }
}
