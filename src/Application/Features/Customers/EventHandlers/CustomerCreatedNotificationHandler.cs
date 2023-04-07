using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events.CustomerEvents;
using Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Customers.EventHandlers;

/// <summary>
/// Classe responsável por tratar a notificação do evento de cliente criado.
/// </summary>
public class CustomerCreatedNotificationHandler : INotificationHandler<CustomerCreatedEventNotification>
{
    private readonly IAppQueryDbContext _queryDbContext;
    private readonly IDbOperationConfiguration _dbOperationConfiguration;

    public CustomerCreatedNotificationHandler(
        IAppQueryDbContext queryDbContext,
        IDbOperationConfiguration dbOperationConfiguration)
    {
        _queryDbContext = queryDbContext;
        _dbOperationConfiguration = dbOperationConfiguration;
    }

    /// <summary>
    /// Método que trata a notificação do evento de cliente criado.
    /// </summary>
    /// <param name="notification">A notificação contendo informações do evento de cliente criado.</param>
    /// <param name="cancellationToken">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    public async Task Handle(CustomerCreatedEventNotification notification, CancellationToken cancellationToken)
    {
        if (_dbOperationConfiguration.UseSingleDatabase()) return;

#pragma warning disable CA1062 // Validate arguments of public methods
        Customer customer = new Customer(notification.Name, notification.Email)
        {
            Created = notification.EventDateTime
        };
#pragma warning restore CA1062 // Validate arguments of public methods

        // Insere o cliente no contexto de leitura
        await _queryDbContext.Customers.AddAsync(customer, cancellationToken).ConfigureAwait(false);
        await _queryDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
