using Application.Common.Cache;
using Application.Common.Interfaces;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Customers.Queries;

public sealed class GetCustomersByEmailQuery : CacheableQueryBase, IRequest<IEnumerable<CustomerSummaryResponse>>, ICacheable
{
    public GetCustomersByEmailQuery(string email)
    {
        Email = email;
    }

    public string Email { get; }

    public override string CacheKey => $"Customer:Email:{Email}";
}
