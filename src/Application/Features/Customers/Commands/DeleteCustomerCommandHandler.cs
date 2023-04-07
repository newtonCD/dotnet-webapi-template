using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Throw;

namespace Application.Features.Customers.Commands;

/// <summary>
/// DeleteCustomerCommandHandler é responsável por processar o comando DeleteCustomerCommand.
/// </summary>
public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância da classe DeleteCustomerCommandHandler.
    /// </summary>
    /// <param name="customerRepository">Repositório de clientes.</param>
    /// <param name="unitOfWork">Unidade de trabalho.</param>
    /// <param name="mediator">Mediador.</param>
    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    /// <summary>
    /// Processa a requisição de exclusão de um cliente.
    /// </summary>
    /// <param name="request">O comando DeleteCustomerCommand contendo as informações do cliente a ser excluído.</param>
    /// <param name="cancellationToken">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(() => throw new ArgumentNullException(nameof(request)));

        Customer customer = await _customerRepository.GetByIdAsync(request.Id).ConfigureAwait(false);
        customer.ThrowIfNull(() => throw new NotFoundException($"Cliente com o ID {request.Id} não encontrado."));
        customer.AddCustomerDeletedEvent();

        _customerRepository.Delete(customer);

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