using Application.Common.Cache;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Customers.Queries;

public sealed class GetCustomerByIdQuery : CacheableQueryBase, IRequest<CustomerResponse>, ICacheable
{
    public GetCustomerByIdQuery(int customerId)
    {
        CustomerId = customerId;
    }

    public int CustomerId { get; }

    public override string CacheKey => $"Customer:{CustomerId}";
}
