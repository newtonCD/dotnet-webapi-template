using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Behaviors;
using Template.Application.Common.Interfaces;

namespace Template.Application.Tests.Behaviors;

public class PollyRetryBehaviorTests
{
    [Fact]
    public async Task Handle_ExecutesPolicy_AndCallsNextHandler()
    {
        // Arrange
        var policyMock = new Mock<IAsyncRetryPolicy>();
        var pollyPoliciesMock = new Mock<IPollyPolicies>();
        var requestHandlerDelegateMock = new Mock<RequestHandlerDelegate<string>>();
        var requestMock = new Mock<IRequest<string>>();

        pollyPoliciesMock.Setup(p => p.HandleDatabaseExceptions()).Returns(policyMock.Object);
        requestHandlerDelegateMock.Setup(d => d()).ReturnsAsync("TestResponse");

        policyMock.Setup(p => p.ExecuteAsync(
            It.IsAny<Func<CancellationToken, Task<string>>>(),
            It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task<string>> action, CancellationToken cancellationToken) => action(cancellationToken));

        var behavior = new PollyRetryBehavior<IRequest<string>, string>(pollyPoliciesMock.Object);

        // Act
        var result = await behavior.Handle(requestMock.Object, requestHandlerDelegateMock.Object, CancellationToken.None);

        // Assert
        pollyPoliciesMock.Verify(p => p.HandleDatabaseExceptions(), Times.Once);
        policyMock.Verify(p => p.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<string>>>(), CancellationToken.None), Times.Once);
        requestHandlerDelegateMock.Verify(d => d(), Times.Once);

        Assert.Equal("TestResponse", result);
    }
}