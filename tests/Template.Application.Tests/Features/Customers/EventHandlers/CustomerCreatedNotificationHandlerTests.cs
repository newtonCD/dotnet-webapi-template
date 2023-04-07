using Application.Common;
using Application.Features.Customers.EventHandlers;
using Domain.Entities;
using Domain.Events.CustomerEvents;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Application.Tests.Features.Customers.EventHandlers;

public class CustomerCreatedNotificationHandlerTests
{
    private readonly Mock<IAppQueryDbContext> _queryDbContextMock;
    private readonly TestDatabaseConfiguration _dbConfiguration;
    private readonly DbSet<Customer> _customerDbSetMock;

    public CustomerCreatedNotificationHandlerTests()
    {
        _queryDbContextMock = new Mock<IAppQueryDbContext>();
        _dbConfiguration = new TestDatabaseConfiguration();
        _customerDbSetMock = DbContextMock.GetQueryableMockDbSet<Customer>();
        _queryDbContextMock.Setup(q => q.Customers).Returns(_customerDbSetMock);
    }

    [Fact]
    public async Task Handle_WhenUsingSingleDatabase_DoesNotAddCustomerToQueryDbContext()
    {
        // Arrange
        _dbConfiguration.UseSingleDatabaseValue = true;

        var handler = new CustomerCreatedNotificationHandler(_queryDbContextMock.Object, _dbConfiguration);
        var notification = new CustomerCreatedEventNotification(1, "John Doe", "john.doe@example.com", DateTime.UtcNow);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        _queryDbContextMock.Verify(q => q.Customers.AddAsync(It.IsAny<Customer>(), CancellationToken.None), Times.Never);
        _queryDbContextMock.Verify(q => q.SaveChangesAsync(CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNotUsingSingleDatabase_AddsCustomerToQueryDbContextAndSavesChanges()
    {
        // Arrange
        _dbConfiguration.UseSingleDatabaseValue = false;

        var handler = new CustomerCreatedNotificationHandler(_queryDbContextMock.Object, _dbConfiguration);
        var notification = new CustomerCreatedEventNotification(1, "John Doe", "john.doe@example.com", DateTime.UtcNow);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        _queryDbContextMock.Verify(q => q.Customers.AddAsync(It.IsAny<Customer>(), CancellationToken.None), Times.Once);
        _queryDbContextMock.Verify(q => q.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}