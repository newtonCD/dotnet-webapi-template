using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Template.Domain.Common;
using Template.Domain.Events.CustomerEvents;

namespace Template.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    private readonly List<INotification> _domainEvents = new();

    public Customer(string name, string email)
        : base()
    {
        Name = name;
        Email = email;
    }

    public Customer(int id, string name, string email)
     : base(id)
    {
        Name = name;
        Email = email;
    }

    public Customer()
    {
    }

    public IReadOnlyCollection<INotification> DomainEvents => new ReadOnlyCollection<INotification>(_domainEvents);

    public string Name { get; private set; }
    public string Email { get; private set; }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public void AddCustomerCreatedEvent()
    {
        _domainEvents.Add(new CustomerCreatedEventNotification(Id, Name, Email, DateTime.UtcNow));
    }

    public void AddCustomerDeletedEvent()
    {
        _domainEvents.Add(new CustomerDeletedEventNotification(Id, DateTime.UtcNow));
    }

    public void AddCustomerUpdatedEvent()
    {
        _domainEvents.Add(new CustomerUpdatedEventNotification(Id, Name, Email, DateTime.UtcNow));
    }
}