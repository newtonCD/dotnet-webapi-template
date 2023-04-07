using Domain.Entities;
using Domain.Events.CustomerEvents;
using System.Linq;

namespace Template.Domain.Tests.Entities;

public class CustomerTests
{
    [Fact]
    public void Customer_Constructor_SetsProperties()
    {
        // Arrange
        string name = "John Doe";
        string email = "john.doe@example.com";

        // Act
        var customer = new Customer(name, email);

        // Assert
        Assert.Equal(name, customer.Name);
        Assert.Equal(email, customer.Email);
    }

    [Fact]
    public void Customer_Update_UpdatesProperties()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        string updatedName = "Jane Doe";
        string updatedEmail = "jane.doe@example.com";

        // Act
        customer.Update(updatedName, updatedEmail);

        // Assert
        Assert.Equal(updatedName, customer.Name);
        Assert.Equal(updatedEmail, customer.Email);
    }

    [Fact]
    public void AddCustomerCreatedEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");

        // Act
        customer.AddCustomerCreatedEvent();

        // Assert
        Assert.Single(customer.DomainEvents);
        Assert.IsType<CustomerCreatedEventNotification>(customer.DomainEvents.First());
    }

    [Fact]
    public void AddCustomerDeletedEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");

        // Act
        customer.AddCustomerDeletedEvent();

        // Assert
        Assert.Single(customer.DomainEvents);
        Assert.IsType<CustomerDeletedEventNotification>(customer.DomainEvents.First());
    }

    [Fact]
    public void AddCustomerUpdatedEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");

        // Act
        customer.AddCustomerUpdatedEvent();

        // Assert
        Assert.Single(customer.DomainEvents);
        Assert.IsType<CustomerUpdatedEventNotification>(customer.DomainEvents.First());
    }
}
