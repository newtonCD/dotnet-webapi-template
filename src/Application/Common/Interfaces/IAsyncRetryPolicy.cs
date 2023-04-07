using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IAsyncRetryPolicy
{
    Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken);
}
