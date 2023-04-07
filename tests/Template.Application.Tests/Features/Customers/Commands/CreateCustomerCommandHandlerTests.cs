using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Customers.Commands;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Moq;
using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Application.Tests.Features.Customers.Commands;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IPollyPolicies> _pollyPoliciesMock;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mediatorMock = new Mock<IMediator>();
        _pollyPoliciesMock = new Mock<IPollyPolicies>();

        var asyncRetryPolicy = Policy.Handle<Exception>().RetryAsync(0);
        _pollyPoliciesMock.Setup(p => p.HandleDatabaseExceptions())
            .Returns(new AsyncRetryPolicyWrapper(asyncRetryPolicy));

        _handler = new CreateCustomerCommandHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateCustomer()
    {
        // Arrange
        var name = "Test Customer";
        var email = "test@example.com";
        var createCustomerCommand = new CreateCustomerCommand(name, email);
        var cancellationToken = CancellationToken.None;

        _customerRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Customer>()))
           .Callback<Customer>(c => c.SetPrivatePropertyValue("Id", 1))
           .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(cancellationToken))
            .Returns(Task.FromResult(1));

        // Act
        Result<int> result = await _handler.Handle(createCustomerCommand, cancellationToken);
        int customerId = result.Data;

        // Assert
        Assert.True(result.Succeeded);
        _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
        _mediatorMock.Verify(x => x.Publish(It.IsAny<INotification>(), cancellationToken), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);

        Assert.NotEqual(0, customerId);
    }
}
