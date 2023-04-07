namespace Application.Features.Customers.Queries;

/// <summary>
/// Classe que representa a resposta resumida de um cliente.
/// </summary>
public sealed record CustomerSummaryResponse(int Id, string Name, string Email);
