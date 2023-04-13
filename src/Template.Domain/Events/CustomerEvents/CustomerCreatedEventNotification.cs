using MediatR;
using System;

namespace Template.Domain.Events.CustomerEvents;

public class CustomerCreatedEventNotification : INotification
{
    public CustomerCreatedEventNotification(int customerId, string name, string email, DateTime eventDateTime)
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