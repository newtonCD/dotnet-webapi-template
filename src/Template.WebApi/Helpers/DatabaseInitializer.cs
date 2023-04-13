using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Template.Domain.Entities;
using Template.Infrastructure.Persistance;

namespace Template.WebApi.Helpers;

/// <summary>
/// Essa classe é utilizada apenas para efeitos de demonstração para popular registros fakes no banco de dados.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DatabaseInitializer
{
    public static void Initialize(AppCommandDbContext commandContext, AppQueryDbContext queryContext)
    {
        if (queryContext is null)
        {
            throw new ArgumentNullException(nameof(queryContext));
        }

        if (commandContext is null)
        {
            throw new ArgumentNullException(nameof(commandContext));
        }

        // Verifica se a tabela Customer já contém registros
        if (commandContext.Customers.Any())
        {
            return;
        }

        if (queryContext.Customers.Any())
        {
            return;
        }

        // Cria alguns registros fictícios
        const int numberOfCustomers = 100;
        var customers = new List<Customer>(numberOfCustomers);

        for (int i = 1; i <= numberOfCustomers; i++)
        {
            customers.Add(new Customer($"Cliente {i}", $"cliente_{i}@email.com"));
        }

        // Adicione os registros fictícios ao banco de dados e salve as alterações
        commandContext.Customers.AddRange(customers);
        commandContext.SaveChanges();

        queryContext.Customers.AddRange(customers);
        queryContext.SaveChanges();
    }
}
