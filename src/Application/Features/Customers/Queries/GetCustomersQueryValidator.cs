using Application.Features.Base;

namespace Application.Features.Customers.Queries;

/// <summary>
/// Classe que valida a consulta para obter todos os clientes.
/// </summary>
public sealed class GetCustomersQueryValidator : GetPagingQueryValidator<GetCustomersQuery>
{
}