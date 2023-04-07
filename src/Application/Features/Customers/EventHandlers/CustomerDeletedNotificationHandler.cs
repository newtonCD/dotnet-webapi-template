using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events.CustomerEvents;
using Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Customers.EventHandlers;

/// <summary>
/// Classe responsável por tratar a notificação do evento de cliente excluído.
/// </summary>
public class CustomerDeletedNotificationHandler : INotificationHandler<CustomerDeletedEventNotification>
{
    private readonly IAppQueryDbContext _queryDbContext;
    private readonly IDbOperationConfiguration _dbConfiguration;
    private readonly ICacheService _cacheService;

    public CustomerDeletedNotificationHandler(
        IAppQueryDbContext readDbContext,
        IDbOperationConfiguration dbConfiguration,
        ICacheService cacheService)
    {
        _queryDbContext = readDbContext;
        _dbConfiguration = dbConfiguration;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Método que trata a notificação do evento de cliente excluído.
    /// </summary>
    /// <param name="notification">A notificação contendo informações do evento de cliente excluído.</param>
    /// <param name="cancellationToken">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    public async Task Handle(CustomerDeletedEventNotification notification, CancellationToken cancellationToken)
    {
        if (_dbConfiguration.UseSingleDatabase()) return;

#pragma warning disable CA1062 // Validate arguments of public methods
        Customer customer = await _queryDbContext.Customers.FindAsync(new object[] { notification.CustomerId }, cancellationToken: cancellationToken).ConfigureAwait(false);
#pragma warning restore CA1062 // Validate arguments of public methods

        if (customer != null)
        {
            // Exclui o cliente no contexto de leitura
            _queryDbContext.Customers.Remove(customer);
            await _queryDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Invalida o cache para o registro excluído
            string cacheKey = $"Customer:{notification.CustomerId}";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
        }
    }
}
