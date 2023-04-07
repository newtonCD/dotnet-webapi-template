using MediatR;
using System;

namespace Domain.Events.CustomerEvents;

public class CustomerUpdatedEventNotification : INotification
{
    public CustomerUpdatedEventNotification(int customerId, string name, string email, DateTime eventDateTime)
    {
        CustomerId = customerId;
        Name = name;
        Email = email;
        EventDateTime = eventDateTime;
    }

    public int CustomerId { get; }
    public string Name { get; }
    public string Email { get; }
    public DateTime EventDateTime { get; }
}