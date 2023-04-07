﻿using Application.Common.Models;
using MediatR;

namespace Application.Features.Customers.Commands;

/// <summary>
/// CreateCustomerCommand representa um comando para criar um novo cliente.
/// </summary>
public sealed record CreateCustomerCommand(string Name, string Email) : IRequest<Result<int>>;
