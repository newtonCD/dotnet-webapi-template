using System.Collections.ObjectModel;

namespace Application.Features.Customers.Queries;

/// <summary>
/// Classe que representa a resposta paginada de clientes.
/// </summary>
public sealed record PagedCustomerResponse(int PageNumber, int PageSize, int TotalPages, int TotalItems, Collection<CustomerSummaryResponse> Items);
