using Application.Common.Interfaces;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviors;

[ExcludeFromCodeCoverage]
public class PollyRetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IPollyPolicies _pollyPolicies;

    public PollyRetryBehavior(IPollyPolicies pollyPolicies)
    {
        _pollyPolicies = pollyPolicies;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return await _pollyPolicies.HandleDatabaseExceptions().ExecuteAsync(async _ => await next().ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
    }
}