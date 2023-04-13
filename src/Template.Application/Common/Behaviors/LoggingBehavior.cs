using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Extensions;
using Template.Application.Settings;
using Throw;

namespace Template.Application.Common.Behaviors;

[ExcludeFromCodeCoverage]
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly LoggingSettings _loggingSettings;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, IOptions<LoggingSettings> loggingSettings)
    {
        loggingSettings.ThrowIfNull(() => throw new ArgumentNullException(nameof(loggingSettings)));

        _logger = logger;
        _loggingSettings = loggingSettings.Value;
    }

    [SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        next.ThrowIfNull(() => throw new ArgumentNullException(nameof(next)));

        TRequest requestCopy = default;
        TResponse responseCopy = default;

        if (_loggingSettings.LogRequestEnabled)
        {
            // Cria uma cópia profunda do objeto de requisição.
            requestCopy = request.DeepCopy();

            // Se necessário, modifique ou remova informações confidenciais do objeto request copiado.
            // Por exemplo:
            // requestCopy.Password = null;
        }

        TResponse response = await next().ConfigureAwait(false);

        if (_loggingSettings.LogResponseEnabled)
        {
            // Cria uma cópia profunda do objeto de resposta
            responseCopy = response.DeepCopy();

            // Se necessário, modifique ou remova informações confidenciais do objeto response copiado.
            // Por exemplo:
            // responseCopy.Token = null;
        }

        // Fazer log da cópia do objeto de requisição e de resposta juntos.
        LoggingBehaviorHelper.LogRequestResponse(_logger, typeof(TRequest).Name, requestCopy, responseCopy, null);

        return response;
    }
}