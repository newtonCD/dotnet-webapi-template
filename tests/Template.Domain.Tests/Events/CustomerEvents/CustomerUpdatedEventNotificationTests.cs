﻿using System;
using Template.Domain.Events.CustomerEvents;

namespace Template.Domain.Tests.Events.CustomerEvents;

public class CustomerUpdatedEventNotificationTests
{
    [Fact]
    public void CustomerUpdatedEventNotification_Constructor_SetsValues()
    {
        // Arrange
        int customerId = 1;
        string name = "John Doe";
        string email = "john.doe@example.com";
        DateTime eventDateTime = DateTime.UtcNow;

        // Act
        var eventNotification = new CustomerUpdatedEventNotification(customerId, name, email, eventDateTime);

        // Assert
        Assert.Equal(customerId, eventNotification.CustomerId);
        Assert.Equal(name, eventNotification.Name);
        Assert.Equal(email, eventNotification.Email);
        Assert.Equal(eventDateTime, eventNotification.EventDateTime);
    }
}
