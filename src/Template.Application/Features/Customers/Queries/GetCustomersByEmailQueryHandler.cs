using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Template.Domain.Entities;
using Template.Domain.Interfaces;

namespace Template.Application.Features.Customers.Queries;

public sealed class GetCustomersByEmailQueryHandler : IRequestHandler<GetCustomersByEmailQuery, IEnumerable<CustomerSummaryResponse>>
{
    private readonly IBaseRepository<Customer> _customerRepository;

    public GetCustomersByEmailQueryHandler(IBaseRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerSummaryResponse>> Handle(GetCustomersByEmailQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Customer, bool>> predicate = customer => customer.Email == request.Email;
        IEnumerable<Customer> customers = await _customerRepository.FindAsync(predicate).ConfigureAwait(false);

        return customers.Adapt<IEnumerable<CustomerSummaryResponse>>();
    }
}