using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Application.Common.Exceptions;
using Template.WebApi.Presenters;

namespace Template.WebApi.Filters;

public sealed class ApiExceptionHandlingFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Func<ExceptionContext, Task>> _exceptionHandlers;

    public ApiExceptionHandlingFilterAttribute()
    {
        _exceptionHandlers = new Dictionary<Type, Func<ExceptionContext, Task>>
        {
            { typeof(BadRequestException), HandleBadRequestExceptionAsync },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessExceptionAsync },
            { typeof(InternalServerException), HandleInternalServerExceptionAsync },
            { typeof(NotFoundException), HandleNotFoundExceptionAsync },
            { typeof(UnauthorizedException), HandleUnauthorizedAccessExceptionAsync },
            { typeof(ValidationException), HandleValidationExceptionAsync },
            { typeof(DbUpdateConcurrencyException), HandleDbUpdateExceptionAsync },
            { typeof(DbUpdateException), HandleDbUpdateExceptionAsync },
            { typeof(CouldNotHandleException), HandleCouldNotHandleExceptionAsync }
        };
    }

    public override async Task OnExceptionAsync(ExceptionContext context)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        await HandleExceptionAsync(context).ConfigureAwait(false);
#pragma warning restore CA1062 // Validate arguments of public methods

        await base.OnExceptionAsync(context).ConfigureAwait(false);
    }

    private async Task HandleExceptionAsync(ExceptionContext context)
    {
        Type type = context.Exception.GetType();

        if (_exceptionHandlers.TryGetValue(type, out Func<ExceptionContext, Task> handler))
        {
            await handler.Invoke(context).ConfigureAwait(false);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            await HandleInvalidModelStateExceptionAsync(context).ConfigureAwait(false);
            return;
        }

        // Se chegou aqui, não foi possível lidar com a exceção.
        Exception exception = context.Exception;
        string message = $"Erro não tratado na API: {exception.Message}";

        ILogger logger = context?.HttpContext.RequestServices.GetService(typeof(ILogger<ApiExceptionHandlingFilterAttribute>)) as ILogger;

        if (logger != null || logger.IsEnabled(LogLevel.Error))
#pragma warning disable CA1848 // Use the LoggerMessage delegates
            logger.LogError(exception, "{Message}", message);
#pragma warning restore CA1848 // Use the LoggerMessage delegates

        context.ExceptionHandled = true;
        await Task.CompletedTask.ConfigureAwait(false);

        throw new CouldNotHandleException(message, exception);
    }

    private static async Task HandleInvalidModelStateExceptionAsync(ExceptionContext context)
    {
        ValidationProblemDetails details = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task HandleBadRequestExceptionAsync(ExceptionContext context)
    {
        BadRequestException exception = (BadRequestException)context.Exception;

        CustomProblemDetails details = new CustomProblemDetails(new List<string> { exception.Message })
        {
            Detail = exception.Message,
            Status = StatusCodes.Status400BadRequest,
            Title = "Requisição mal formatada.",
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task HandleForbiddenAccessExceptionAsync(ExceptionContext context)
    {
        ForbiddenAccessException exception = (ForbiddenAccessException)context.Exception;

        CustomProblemDetails details = new CustomProblemDetails(new List<string> { exception.Message })
        {
            Detail = exception.Message,
            Status = StatusCodes.Status403Forbidden,
            Title = "Acesso proibido.",
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.3",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status403Forbidden,
        };

        context.ExceptionHandled = true;

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task HandleNotFoundExceptionAsync(ExceptionContext context)
    {
        NotFoundException exception = (NotFoundException)context.Exception;

        CustomProblemDetails details = new CustomProblemDetails(new List<string> { exception.Message })
        {
            Detail = exception.Message,
            Status = StatusCodes.Status404NotFound,
            Title = "Recurso não encontrado.",
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.4",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new NotFoundObjectResult(details);
        context.ExceptionHandled = true;

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task HandleUnauthorizedAccessExceptionAsync(ExceptionContext context)
    {
        UnauthorizedException exception = (UnauthorizedException)context.Exception;

        CustomProblemDetails details = new CustomProblemDetails(new List<string> { exception.Message })
        {
            Detail = exception.Message,
            Status = StatusCodes.Status401Unauthorized,
            Title = "Acesso não autorizado.",
            Type = "https://www.rfc-editor.org/rfc/rfc7235#section-3.1",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new UnauthorizedObjectResult(details);
        context.ExceptionHandled = true;

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task HandleValidationExceptionAsync(ExceptionContext context)
    {
        ValidationException exception = (ValidationException)context.Exception;

        ValidationProblemDetails details = new ValidationProblemDetails(exception.Errors)
        {
            Title = "Erro de validação.",
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task HandleInternalServerExceptionAsync(ExceptionContext context)
    {
        InternalServerException exception = (InternalServerException)context.Exception;

        CustomProblemDetails details = new CustomProblemDetails(new List<string> { exception.Message })
        {
            Detail = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro interno do servidor.",
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };

        context.ExceptionHandled = true;

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task HandleDbUpdateExceptionAsync(ExceptionContext context)
    {
        var dbUpdateException = (DbUpdateException)context.Exception;
        var newException = new CustomDbUpdateException(dbUpdateException.Message, dbUpdateException.InnerException);

        CustomProblemDetails details = new CustomProblemDetails(new List<string> { newException.Message })
        {
            Detail = newException.Message,
            Status = StatusCodes.Status400BadRequest,
            Title = "Erro (EF) no banco de dados.",
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task HandleCouldNotHandleExceptionAsync(ExceptionContext context)
    {
        CouldNotHandleException exception = (CouldNotHandleException)context.Exception;

        CustomProblemDetails details = new CustomProblemDetails(new List<string> { exception.Message })
        {
            Detail = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Title = "Ocorreu um erro não tratado.",
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };

        context.ExceptionHandled = true;

        await Task.CompletedTask.ConfigureAwait(false);
    }
}