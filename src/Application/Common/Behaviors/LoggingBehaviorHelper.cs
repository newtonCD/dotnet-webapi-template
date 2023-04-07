using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Behaviors;

[ExcludeFromCodeCoverage]
public static class LoggingBehaviorHelper
{
    public static readonly Action<ILogger, string, object, Exception> LogRequestStart =
        LoggerMessage.Define<string, object>(
            LogLevel.Information,
            new EventId(1, "RequestStart"),
            "Início requisição {RequestName}: {@Request}");

    public static readonly Action<ILogger, string, Exception> LogRequestEnd =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(2, "RequestEnd"),
            "Fim requisição {RequestName}");

    public static readonly Action<ILogger, string, object, Exception> LogResponse =
        LoggerMessage.Define<string, object>(
            LogLevel.Information,
            new EventId(3, "Response"),
            "Resposta {RequestName}: {@Response}");

    public static readonly Action<ILogger, string, object, object, Exception> LogRequestResponse =
        LoggerMessage.Define<string, object, object>(
            LogLevel.Information,
            new EventId(4, "RequestResponse"),
            "Request: {RequestName} {@Request} - Response: {@Response}");
}
