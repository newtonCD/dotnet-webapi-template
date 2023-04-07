using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;

namespace WebApi;

[ExcludeFromCodeCoverage]
public class Program
{
    protected Program()
    {
    }

    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                .CreateBootstrapLogger();

        Log.Information("Starting up...");

        try
        {
            IdentityModelEventSource.ShowPII = true;

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Host.Configure();
            builder.Services.ConfigureServices(builder.Configuration);

            WebApplication app = builder.Build();
            app.ConfigureWebApplication(app.Services);

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly.");
        }
        finally
        {
            Log.Information("Server shutting down...");
            Log.CloseAndFlush();
        }
    }
}