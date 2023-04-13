using MediatR;
using System.Collections.Generic;
using Template.Application.Common.Cache;
using Template.Application.Common.Interfaces;

namespace Template.Application.Features.Customers.Queries;

public sealed class GetCustomersByEmailQuery : CacheableQueryBase, IRequest<IEnumerable<CustomerSummaryResponse>>, ICacheable
{
    public GetCustomersByEmailQuery(string email)
    {
        Email = email;
    }

    public string Email { get; }

    public override string CacheKey => $"Customer:Email:{Email}";
}
