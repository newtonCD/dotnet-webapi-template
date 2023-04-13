using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common;
using Template.Application.Common.Interfaces;
using Template.Application.Features.Customers.EventHandlers;
using Template.Domain.Entities;
using Template.Domain.Events.CustomerEvents;
using Template.Domain.Interfaces;

namespace Template.Application.Tests.Features.Customers.EventHandlers;

public class CustomerDeletedNotificationHandlerTests
{
    [Fact]
    public async Task Handle_SingleDatabaseConfiguration_ShouldNotDeleteCustomerFromQueryContext()
    {
        // Arrange
        var queryDbContextMock = new Mock<IAppQueryDbContext>();
        var dbConfiguration = new DbOperationConfiguration
        {
            CommandConnectionString = "connection_string",
            QueryConnectionString = "connection_string"
        };

        var cacheServiceMock = new Mock<ICacheService>();
        var handler = new CustomerDeletedNotificationHandler(queryDbContextMock.Object, dbConfiguration, cacheServiceMock.Object);
        var notification = new CustomerDeletedEventNotification(1, DateTime.UtcNow);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        queryDbContextMock.Verify(ctx => ctx.Customers.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Never);
        queryDbContextMock.Verify(ctx => ctx.Customers.Remove(It.IsAny<Customer>()), Times.Never);
        queryDbContextMock.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MultipleDatabaseConfiguration_ShouldDeleteCustomerFromQueryContext()
    {
        // Arrange
        var queryDbContextMock = new Mock<IAppQueryDbContext>();
        var dbConfiguration = new DbOperationConfiguration
        {
            CommandConnectionString = "command_connection_string",
            QueryConnectionString = "query_connection_string"
        };

        var cacheServiceMock = new Mock<ICacheService>();
        var handler = new CustomerDeletedNotificationHandler(queryDbContextMock.Object, dbConfiguration, cacheServiceMock.Object);
        var notification = new CustomerDeletedEventNotification(1, DateTime.UtcNow);

        Customer customer = new Customer("John Doe", "john.doe@example.com");
        typeof(Customer).GetProperty("Id")?.SetValue(customer, 1);

        queryDbContextMock
            .Setup(ctx => ctx.Customers.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<Customer>(customer));

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        queryDbContextMock.Verify(ctx => ctx.Customers.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        queryDbContextMock.Verify(ctx => ctx.Customers.Remove(It.IsAny<Customer>()), Times.Once);
        queryDbContextMock.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}
