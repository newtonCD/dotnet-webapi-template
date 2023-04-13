using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Exceptions;
using Template.Domain.Entities;
using Template.Domain.Interfaces;
using Throw;

namespace Template.Application.Features.Customers.Commands;

/// <summary>
/// UpdateCustomerCommandHandler é responsável por processar o comando UpdateCustomerCommand.
/// </summary>
public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância da classe UpdateCustomerCommandHandler.
    /// </summary>
    /// <param name="customerRepository">Repositório de clientes.</param>
    /// <param name="unitOfWork">Unidade de trabalho.</param>
    /// <param name="mediator">Mediador.</param>
    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    /// <summary>
    /// Processa a requisição de atualização de um cliente.
    /// </summary>
    /// <param name="request">O comando UpdateCustomerCommand contendo as informações do cliente a ser atualizado.</param>
    /// <param name="cancellationToken">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    /// <returns>Um objeto Result que representa a conclusão bem-sucedida da operação.</returns>
    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(() => throw new ArgumentNullException(nameof(request)));

        Customer customer = await _customerRepository.GetByIdAsync(request.Id).ConfigureAwait(false);
        customer.ThrowIfNull(() => throw new NotFoundException($"Cliente com o ID {request.Id} não encontrado."));
        customer.Update(request.Name, request.Email);
        customer.AddCustomerUpdatedEvent();

        _customerRepository.Update(customer);

        if (customer.DomainEvents != null)
        {
            foreach (var domainEvent in customer.DomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
