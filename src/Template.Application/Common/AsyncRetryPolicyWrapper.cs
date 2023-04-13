using Polly.Retry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Interfaces;

namespace Template.Application.Common;

[ExcludeFromCodeCoverage]
public class AsyncRetryPolicyWrapper : IAsyncRetryPolicy
{
    private readonly AsyncRetryPolicy _policy;

    public AsyncRetryPolicyWrapper(AsyncRetryPolicy policy)
    {
        _policy = policy;
    }

    public Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
    {
        return _policy.ExecuteAsync(action, cancellationToken);
    }
}