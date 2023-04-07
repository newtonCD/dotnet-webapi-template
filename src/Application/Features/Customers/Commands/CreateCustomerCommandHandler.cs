using Application.Common.Models;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Throw;

namespace Application.Features.Customers.Commands;

/// <summary>
/// CreateCustomerCommandHandler é responsável por lidar com a criação de um novo cliente.
/// </summary>
public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<int>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância da classe CreateCustomerCommandHandler.
    /// </summary>
    /// <param name="customerRepository">Repositório de clientes.</param>
    /// <param name="unitOfWork">Unidade de trabalho.</param>
    /// <param name="mediator">Mediador.</param>
    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    /// <summary>
    /// Trata o comando de criação de um novo cliente.
    /// </summary>
    /// <param name="request">Comando para criar um novo cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Retorna o ID do cliente criado.</returns>
    public async Task<Result<int>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(() => throw new ArgumentNullException(nameof(request)));

        Customer customer = new Customer(request.Name, request.Email);
        customer.AddCustomerCreatedEvent();

        await _customerRepository.AddAsync(customer).ConfigureAwait(false);

        if (customer.DomainEvents != null)
        {
            foreach (var domainEvent in customer.DomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return ResultFactory.Success(customer.Id);
    }
}