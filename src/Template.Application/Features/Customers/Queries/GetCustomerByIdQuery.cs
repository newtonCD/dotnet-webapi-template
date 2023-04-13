using MediatR;
using Template.Application.Common.Cache;
using Template.Application.Common.Interfaces;

namespace Template.Application.Features.Customers.Queries;

public sealed class GetCustomerByIdQuery : CacheableQueryBase, IRequest<CustomerResponse>, ICacheable
{
    public GetCustomerByIdQuery(int customerId)
    {
        CustomerId = customerId;
    }

    public int CustomerId { get; }

    public override string CacheKey => $"Customer:{CustomerId}";
}
