using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using Mapster;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Throw;

namespace Application.Features.Customers.Queries;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerResponse>
{
    private readonly IBaseRepository<Customer> _customerRepository;

    public GetCustomerByIdQueryHandler(IBaseRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerResponse> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        Customer customer = await _customerRepository.GetByIdAsync(request.CustomerId).ConfigureAwait(false);
#pragma warning restore CA1062 // Validate arguments of public methods
        customer.ThrowIfNull(() => throw new NotFoundException($"Cliente com o ID {request.CustomerId} não encontrado."));

        return customer.Adapt<CustomerResponse>();
    }
}