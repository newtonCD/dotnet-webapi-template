using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Interfaces;
using Template.Domain.Entities;
using Template.Domain.Events.CustomerEvents;
using Template.Domain.Interfaces;

namespace Template.Application.Features.Customers.EventHandlers;

/// <summary>
/// Classe responsável por tratar a notificação do evento de cliente atualizado.
/// </summary>
public class CustomerUpdatedNotificationHandler : INotificationHandler<CustomerUpdatedEventNotification>
{
    private readonly IAppQueryDbContext _queryDbContext;
    private readonly IDbOperationConfiguration _dbConfiguration;
    private readonly ICacheService _cacheService;

    public CustomerUpdatedNotificationHandler(
        IAppQueryDbContext queryDbContext,
        IDbOperationConfiguration dbConfiguration,
        ICacheService cacheService)
    {
        _queryDbContext = queryDbContext;
        _dbConfiguration = dbConfiguration;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Método que trata a notificação do evento de cliente atualizado.
    /// </summary>
    /// <param name="notification">A notificação contendo informações do evento de cliente atualizado.</param>
    /// <param name="cancellationToken">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    public async Task Handle(CustomerUpdatedEventNotification notification, CancellationToken cancellationToken)
    {
        if (_dbConfiguration.UseSingleDatabase()) return;

#pragma warning disable CA1062 // Validate arguments of public methods
        Customer customer = await _queryDbContext.Customers.FindAsync(new object[] { notification.CustomerId }, cancellationToken: cancellationToken).ConfigureAwait(false);
#pragma warning restore CA1062 // Validate arguments of public methods

        if (customer != null)
        {
            customer.Update(notification.Name, notification.Email);

            // Atualize o cliente no contexto de leitura
            _queryDbContext.Customers.Update(customer);
            await _queryDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Invalida o cache para o registro atualizado
            string cacheKey = $"Customer:{notification.CustomerId}";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
        }
    }
}