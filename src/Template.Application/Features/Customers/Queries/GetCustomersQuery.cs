using MediatR;
using Template.Application.Common.Cache;
using Template.Application.Common.Interfaces;
using Template.Application.Features.Base;

namespace Template.Application.Features.Customers.Queries;

/// <summary>
/// Classe de consulta que representa a solicitação para obter todos os clientes.
/// </summary>
public sealed class GetCustomersQuery : CacheableQueryBase, IRequest<PagedCustomerResponse>, IGetPagingQuery, ICacheable
{
    public GetCustomersQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; }
    public int PageSize { get; }

    public override string CacheKey => $"Customer:Paging:{PageNumber}_{PageSize}";
}