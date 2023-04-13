using AspNetCoreRateLimit;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Template.Infrastructure.Persistance;
using Template.WebApi.Helpers;
using Template.WebApi.Middlewares;

namespace Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ConfigureWebApplicationExtensions
{
    public static IApplicationBuilder ConfigureWebApplication(this IApplicationBuilder app, IServiceProvider serviceProvider)
    {
        const string CorsPoliceName = "localhost";

        #region Utilizado para popular registros fake

        // Cria um escopo temporário para resolver o ApplicationDbContext
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppCommandDbContext commandDbContext = scope.ServiceProvider.GetRequiredService<AppCommandDbContext>();
            AppQueryDbContext queryDbContext = scope.ServiceProvider.GetRequiredService<AppQueryDbContext>();

            DatabaseInitializer.Initialize(commandDbContext, queryDbContext);
        }

        #endregion

        app.UseProblemDetails();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            IApiVersionDescriptionProvider provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (string description in provider.ApiVersionDescriptions.Select(x => x.GroupName).Where(x => x != null))
            {
                options.SwaggerEndpoint($"/swagger/{description}/swagger.json", description.ToUpperInvariant());
            }
        });

        app.UseSerilogRequestLogging();

        app.UseRouting();
        app.UseMiddleware<RateLimitResponseMiddleware>();
        app.UseIpRateLimiting();

        app.UseCors(CorsPoliceName);
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHealthChecks("/health");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireCors(CorsPoliceName);
            endpoints.MapHealthChecks("/hc/ready", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("ready"),
            });
            endpoints.MapHealthChecks("/hc/live", new HealthCheckOptions()
            {
                Predicate = (_) => false
            });
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        });

        return app;
    }
}