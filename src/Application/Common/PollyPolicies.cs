using Application.Common.Interfaces;
using Application.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Application.Common;

[ExcludeFromCodeCoverage]
public class PollyPolicies : IPollyPolicies
{
    private readonly PollyPoliciesSettings _pollyPoliciesSettings;

    public PollyPolicies(IOptions<PollyPoliciesSettings> options)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        _pollyPoliciesSettings = options.Value;
#pragma warning restore CA1062 // Validate arguments of public methods
    }

    public virtual IAsyncRetryPolicy HandleDatabaseExceptions()
    {
        AsyncRetryPolicy policy = Policy
            .Handle<DbUpdateException>() // Coloque aqui os tipos de exceções de banco de dados você quer que o Polly aplique as regras de resiliência
            .Or<DbUpdateConcurrencyException>()
            .Or<SqlException>()
            .WaitAndRetryAsync(
                retryCount: _pollyPoliciesSettings.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(_pollyPoliciesSettings.RetryBase, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, _) =>
                {
                    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Warning))
                        Log.Warning($"Atrasando em {timeSpan.TotalSeconds} segundos, para então fazer nova tentativa {retryCount}. Exception: {exception.Message}");
                });

        return new AsyncRetryPolicyWrapper(policy);
    }
}