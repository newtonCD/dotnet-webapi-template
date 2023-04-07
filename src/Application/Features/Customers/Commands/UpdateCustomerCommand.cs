using MediatR;

namespace Application.Features.Customers.Commands;

/// <summary>
/// UpdateCustomerCommand é responsável por representar a requisição de atualização de um cliente.
/// </summary>
public sealed record UpdateCustomerCommand(int Id, string Name, string Email) : IRequest;
