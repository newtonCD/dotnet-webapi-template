using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Customers.EventHandlers;
using Domain.Entities;
using Domain.Events.CustomerEvents;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Template.Application.Tests.Features.Customers.EventHandlers;

public class CustomerUpdatedNotificationHandlerTests
{
    private readonly IAppQueryDbContext _queryDbContext;
    private readonly TestDatabaseConfiguration _dbConfiguration;

    public CustomerUpdatedNotificationHandlerTests()
    {
        var uniqueDatabaseName = $"InMemoryDbForTesting-{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<TestAppQueryDbContext>()
             .UseInMemoryDatabase(databaseName: uniqueDatabaseName)
             .Options;

        _queryDbContext = new TestAppQueryDbContext(options);
        _dbConfiguration = new TestDatabaseConfiguration();

        _dbConfiguration.QueryConnectionString = "QueryConnectionString";
        _dbConfiguration.CommandConnectionString = "CommandConnectionString";
    }

    [Fact]
    public async Task Handle_CustomerUpdated_Notification_ShouldUpdateCustomer()
    {
        // Arrange
        var customerId = 1;
        var name = "Updated Name";
        var email = "updated.email@example.com";
        var eventDateTime = DateTime.UtcNow;

        var customer = new Customer("Original Name", "original.email@example.com").WithId(customerId);

        _queryDbContext.Customers.Add(customer);
        await _queryDbContext.SaveChangesAsync();

        var notification = new CustomerUpdatedEventNotification(customerId, name, email, eventDateTime);

        var cacheServiceMock = new Mock<ICacheService>();

        var handler = new CustomerUpdatedNotificationHandler(_queryDbContext, _dbConfiguration.ToDatabaseConfiguration(), cacheServiceMock.Object);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        var updatedCustomer = _queryDbContext.Customers.FirstOrDefault(x => x.Id == customerId);
        Assert.NotNull(updatedCustomer);
        Assert.Equal(name, updatedCustomer.Name);
        Assert.Equal(email, updatedCustomer.Email);

        await Cleanup();
    }

    [Fact]
    public async Task Handle_CustomerUpdated_Notification_SingleDatabase_ShouldUpdateCustomer()
    {
        // Arrange
        var customerId = 1;
        var name = "Updated Name";
        var email = "updated.email@example.com";
        var eventDateTime = DateTime.UtcNow;

        var customer = new Customer("Original Name", "original.email@example.com").WithId(customerId);

        _queryDbContext.Customers.Add(customer);
        await _queryDbContext.SaveChangesAsync();

        var notification = new CustomerUpdatedEventNotification(customerId, name, email, eventDateTime);

        var cacheServiceMock = new Mock<ICacheService>();

        // Configure TestDatabaseConfiguration para usar um único banco de dados
        _dbConfiguration.UseSingleDatabaseValue = true;

        var handler = new CustomerUpdatedNotificationHandler(_queryDbContext, _dbConfiguration.ToDatabaseConfiguration(), cacheServiceMock.Object);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        var updatedCustomer = _queryDbContext.Customers.FirstOrDefault(x => x.Id == customerId);
        Assert.NotNull(updatedCustomer);
        Assert.Equal(name, updatedCustomer.Name); // Verifique se o nome foi atualizado
        Assert.Equal(email, updatedCustomer.Email); // Verifique se o email foi atualizado

        await Cleanup();
    }

    private async Task Cleanup()
    {
        var customers = _queryDbContext.Customers.ToList();
        _queryDbContext.Customers.RemoveRange(customers);
        await _queryDbContext.SaveChangesAsync();
    }
}
