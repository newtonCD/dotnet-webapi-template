using MediatR;
using System;

namespace Template.Domain.Events.CustomerEvents;

public class CustomerDeletedEventNotification : INotification
{
    public CustomerDeletedEventNotification(int customerId, DateTime occurredOn)
    {
        CustomerId = customerId;
        EventDateTime = occurredOn;
    }

    public int CustomerId { get; }
    public DateTime EventDateTime { get; }
}