using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Interfaces;

namespace Template.Application.Common.Behaviors;

[ExcludeFromCodeCoverage]
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cacheService;
    private readonly ICustomServiceProvider _serviceProvider;

    public CachingBehavior(ICacheService cacheService, ICustomServiceProvider serviceProvider)
    {
        _cacheService = cacheService;
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Verifica se a solicitação implementa a interface ICacheable.
        if (request is not ICacheable cacheableRequest)
#pragma warning disable CA1062 // Validate arguments of public methods
            return await next().ConfigureAwait(false);
#pragma warning restore CA1062 // Validate arguments of public methods

            // Tenta obter o valor do cache.
        TResponse cachedResponse = await _cacheService.GetAsync<TResponse>(cacheableRequest.CacheKey, cancellationToken).ConfigureAwait(false);

        if (cachedResponse != null)
        {
            // Retorna o valor se estiver no cache.
            return cachedResponse;
        }

        // Caso contrário, execute a próxima etapa do pipeline para obter o valor
        TResponse response = await next().ConfigureAwait(false);

        // Armazena o valor no cache
        await _cacheService.SetAsync(
            cacheableRequest.CacheKey,
            response,
            cacheableRequest.GetCacheExpiration(_serviceProvider),
            cancellationToken).ConfigureAwait(false);

        return response;
    }
}
