using MediatR;

namespace Application.Features.Customers.Commands;

/// <summary>
/// DeleteCustomerCommand é responsável por representar a requisição de exclusão de um cliente.
/// </summary>
public sealed record DeleteCustomerCommand(int Id) : IRequest;
