using MediatR;
using Moq;
using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common;
using Template.Application.Common.Exceptions;
using Template.Application.Common.Interfaces;
using Template.Application.Features.Customers.Commands;
using Template.Domain.Entities;
using Template.Domain.Interfaces;

namespace Template.Application.Tests.Features.Customers.Commands;

public class UpdateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IPollyPolicies> _pollyPoliciesMock;

    public UpdateCustomerCommandHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mediatorMock = new Mock<IMediator>();
        _pollyPoliciesMock = new Mock<IPollyPolicies>();

        var asyncRetryPolicy = Policy.Handle<Exception>().RetryAsync(0);
        _pollyPoliciesMock.Setup(p => p.HandleDatabaseExceptions())
            .Returns(new AsyncRetryPolicyWrapper(asyncRetryPolicy));
    }

    [Fact]
    public async Task Handle_WhenCustomerExists_UpdatesCustomer()
    {
        // Arrange
        int customerId = 1;
        string newName = "Updated Name";
        string newEmail = "updated.email@example.com";

        var customer = new Customer(customerId, "Old Name", "old.email@example.com");

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateCustomerCommandHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorMock.Object);
        var command = new UpdateCustomerCommand(customerId, newName, newEmail);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newName, customer.Name);
        Assert.Equal(newEmail, customer.Email);
        _customerRepositoryMock.Verify(repo => repo.Update(customer), Times.Once());
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        int customerId = 1;

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync((Customer)null);

        var handler = new UpdateCustomerCommandHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorMock.Object);
        var command = new UpdateCustomerCommand(customerId, "New Name", "new.email@example.com");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}