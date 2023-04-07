using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
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

public class DeleteCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IPollyPolicies> _pollyPoliciesMock;

    public DeleteCustomerCommandHandlerTests()
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
    public async Task Handle_WhenCustomerExists_DeletesCustomer()
    {
        // Arrange
        int customerId = 1;
        var customer = new Customer(customerId, "John Doe", "test@email.com");

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteCustomerCommandHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorMock.Object);
        var command = new DeleteCustomerCommand(customerId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _customerRepositoryMock.Verify(repo => repo.Delete(customer), Times.Once());
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        int customerId = 1;

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync((Customer)null);

        var handler = new DeleteCustomerCommandHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object, _mediatorMock.Object);
        var command = new DeleteCustomerCommand(customerId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}