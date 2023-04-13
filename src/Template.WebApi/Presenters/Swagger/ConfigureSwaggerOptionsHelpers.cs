using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Template.WebApi.Presenters.Swagger;

[ExcludeFromCodeCoverage]
internal static class ConfigureSwaggerOptionsHelpers
{
    public static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        Assembly assembly = Assembly.GetEntryAssembly();

        OpenApiInfo info = new OpenApiInfo()
        {
            Title = "Template API",
            Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description + "<br /> Assembly version: " + assembly.GetName().Version?.ToString(),
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += " Esta versão da API foi descontinuada. Favor utilizar uma das disponíveis no explorador.";
        }

        return info;
    }
}