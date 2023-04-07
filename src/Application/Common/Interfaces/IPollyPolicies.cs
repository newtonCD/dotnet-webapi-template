using Polly.Retry;

namespace Application.Common.Interfaces;
public interface IPollyPolicies
{
    IAsyncRetryPolicy HandleDatabaseExceptions();
}
