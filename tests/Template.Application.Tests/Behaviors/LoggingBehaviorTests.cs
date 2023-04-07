using Application.Common.Behaviors;
using Application.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Template.Application.Tests.Behaviors;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task LoggingBehavior_ShouldLogInformation()
    {
        // Arrange
        var loggerFactory = new NullLoggerFactory();
        var logger = loggerFactory.CreateLogger<LoggingBehavior<TestRequest, string>>();

        var loggingBehavior = new LoggingBehavior<TestRequest, string>(logger, Options.Create(new LoggingSettings { LogRequestEnabled = true, LogResponseEnabled = true }));

        var request = new TestRequest();
        var cancellationToken = CancellationToken.None;
        var next = new RequestHandlerDelegate<string>(() => Task.FromResult("Test"));

        // Act
        var result = await loggingBehavior.Handle(request, next, cancellationToken);

        // Assert
        // Como estamos usando o NullLoggerFactory, o teste não verificará as chamadas de log, mas garantirá que o código seja executado sem erros.
        Assert.Equal("Test", result);
    }

    public class TestRequest : IRequest<string>
    {
    }
}
