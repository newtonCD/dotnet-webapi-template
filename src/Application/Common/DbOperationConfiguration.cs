using Application.Common.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Application.Common;

/// <summary>
/// Classe para validar as configurações de banco de dados.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class DbOperationConfiguration : IDbOperationConfiguration
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="DbOperationConfiguration"/>.
    /// </summary>
    public DbOperationConfiguration()
    {
    }

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="DbOperationConfiguration"/>.
    /// </summary>
    /// <param name="commandConnectionString"></param>
    /// <param name="queryConnectionString"></param>
    public DbOperationConfiguration(string commandConnectionString, string queryConnectionString)
    {
        CommandConnectionString = commandConnectionString;
        QueryConnectionString = queryConnectionString;
    }

    public string CommandConnectionString { get; set; }
    public string QueryConnectionString { get; set; }

    /// <summary>
    /// Propriedade que verifica se as strings de conexão não estão em branco.
    /// </summary>
    /// <exception cref="InvalidOperationException">Caso uma ou ambas as strings de conexão estiverem vazias.</exception>
    public bool UseSingleDatabase()
    {
        if (string.IsNullOrWhiteSpace(CommandConnectionString) || string.IsNullOrWhiteSpace(QueryConnectionString))
        {
            throw new InvalidOperationException("As strings de conexão de leitura e escrita precisam ser informadas.");
        }

        return string.Equals(CommandConnectionString, QueryConnectionString, StringComparison.OrdinalIgnoreCase);
    }
}