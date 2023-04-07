using Domain.Events.CustomerEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Domain.Tests.Events.CustomerEvents;

public class CustomerDeletedEventNotificationTests
{
    [Fact]
    public void CustomerDeletedEventNotification_Constructor_SetsValues()
    {
        // Arrange
        int customerId = 1;
        DateTime eventDateTime = DateTime.UtcNow;

        // Act
        var eventNotification = new CustomerDeletedEventNotification(customerId, eventDateTime);

        // Assert
        Assert.Equal(customerId, eventNotification.CustomerId);
        Assert.Equal(eventDateTime, eventNotification.EventDateTime);
    }
}