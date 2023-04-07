using Domain.Events.CustomerEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Domain.Tests.Events.CustomerEvents;

public class CustomerCreatedEventNotificationTests
{
    [Fact]
    public void CustomerCreatedEventNotification_Constructor_SetsValues()
    {
        // Arrange
        int customerId = 1;
        string name = "John Doe";
        string email = "john.doe@example.com";
        DateTime eventDateTime = DateTime.UtcNow;

        // Act
        var eventNotification = new CustomerCreatedEventNotification(customerId, name, email, eventDateTime);

        // Assert
        Assert.Equal(customerId, eventNotification.CustomerId);
        Assert.Equal(name, eventNotification.Name);
        Assert.Equal(email, eventNotification.Email);
        Assert.Equal(eventDateTime, eventNotification.EventDateTime);
    }
}