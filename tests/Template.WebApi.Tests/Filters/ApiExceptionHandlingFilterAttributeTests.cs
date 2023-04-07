using Application.Common.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Filters;
using WebApi.Presenters;

namespace Template.WebApi.Tests.Filters;

public class ApiExceptionHandlingFilterAttributeTests
{
    private readonly ApiExceptionHandlingFilterAttribute _filter;
    private readonly Mock<ILogger<ApiExceptionHandlingFilterAttribute>> _loggerMock;

    public ApiExceptionHandlingFilterAttributeTests()
    {
        _loggerMock = new Mock<ILogger<ApiExceptionHandlingFilterAttribute>>();
        _filter = new ApiExceptionHandlingFilterAttribute();
    }

    private ExceptionContext CreateExceptionContext(Exception exception)
    {
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext
            {
                // Adicione o serviço ILogger para evitar um NullReferenceException
                RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider()
            },
            RouteData = new(),
            ActionDescriptor = new()
        };

        return new ExceptionContext(actionContext, Array.Empty<IFilterMetadata>())
        {
            Exception = exception
        };
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleBadRequestException()
    {
        // Arrange
        var exceptionContext = CreateExceptionContext(new BadRequestException("Bad Request"));

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleNotFoundException()
    {
        // Arrange
        var exceptionContext = CreateExceptionContext(new NotFoundException("Not Found"));

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<NotFoundObjectResult>(exceptionContext.Result);
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleUnauthorizedAccessException()
    {
        // Arrange
        var exceptionContext = CreateExceptionContext(new UnauthorizedException("Unauthorized"));

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<UnauthorizedObjectResult>(exceptionContext.Result);
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleForbiddenAccessException()
    {
        // Arrange
        var exceptionContext = CreateExceptionContext(new ForbiddenAccessException("Forbidden"));

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<ObjectResult>(exceptionContext.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, ((ObjectResult)exceptionContext.Result).StatusCode);
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleValidationException()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
                                            {
                                                { "ErrorKey", new[] { "ErrorMessage" } }
                                            };
        var exceptionContext = CreateExceptionContext(new ValidationException("Validation error", errors));

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);

        var badRequestObjectResult = exceptionContext.Result as BadRequestObjectResult;
        var validationProblemDetails = badRequestObjectResult?.Value as ValidationProblemDetails;

        Assert.NotNull(validationProblemDetails);
        Assert.Equal("Erro de validação.", validationProblemDetails.Title);
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleInternalServerException()
    {
        // Arrange
        var exceptionContext = CreateExceptionContext(new InternalServerException("InternalServer"));

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<ObjectResult>(exceptionContext.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, ((ObjectResult)exceptionContext.Result).StatusCode);
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleInvalidModelStateException()
    {
        // Arrange
        var exceptionContext = CreateExceptionContext(new Exception());
        exceptionContext.ModelState.AddModelError("ErrorKey", "ErrorMessage");

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldNotHandleUnhandledException()
    {
        // Arrange
        var exceptionContext = CreateExceptionContext(new Exception("Unhandled exception"));

        // Act & Assert
        await Assert.ThrowsAsync<CouldNotHandleException>(() => _filter.OnExceptionAsync(exceptionContext));
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleDbUpdateConcurrencyException()
    {
        // Arrange
        var exceptionContext = CreateExceptionContext(new DbUpdateConcurrencyException("DbUpdateConcurrency"));

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleDbUpdateException()
    {
        // Arrange
        var exceptionContext = CreateExceptionContext(new DbUpdateException("DbUpdate"));

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldHandleCouldNotHandleException()
    {
        // Arrange
        var innerException = new Exception("Inner exception");
        var exceptionContext = CreateExceptionContext(new CouldNotHandleException("Could not handle exception", innerException));

        // Act
        await _filter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.IsType<ObjectResult>(exceptionContext.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, ((ObjectResult)exceptionContext.Result).StatusCode);

        var objectResult = exceptionContext.Result as ObjectResult;
        var customProblemDetails = objectResult?.Value as CustomProblemDetails;

        Assert.NotNull(customProblemDetails);
        Assert.Equal("Ocorreu um erro não tratado.", customProblemDetails.Title);
    }
}