using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

/// <summary>
/// O padrão Unit of Work é uma prática útil para garantir a consistência e a atomicidade
/// das operações no banco de dados, especialmente em cenários mais complexos.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppCommandDbContext _commandContext;
    private readonly AppQueryDbContext _queryContext;
    private readonly Dictionary<Type, object> _repositories;
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed;

    /// <summary>
    /// Inicializa uma nova instância da classe UnitOfWork.
    /// </summary>
    /// <param name="commandContext">O contexto do banco de dados para operações de gravação.</param>
    /// <param name="queryContext">O contexto do banco de dados para operações de leitura.</param>
    /// <param name="serviceProvider">O ServiceProvider.</param>
    public UnitOfWork(AppCommandDbContext commandContext, AppQueryDbContext queryContext, IServiceProvider serviceProvider)
    {
        _commandContext = commandContext;
        _queryContext = queryContext;
        _repositories = new Dictionary<Type, object>();
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Obtém o repositório para a entidade especificada.
    /// </summary>
    /// <typeparam name="TRepository">O tipo do repositório.</typeparam>
    /// <typeparam name="TEntity">O tipo da entidade.</typeparam>
    /// <returns>Uma instância do repositório.</returns>
    TRepository IUnitOfWork.GetRepository<TRepository, TEntity>()
    {
        Type entityType = typeof(TEntity);

        if (!_repositories.TryGetValue(entityType, out object value))
        {
            object repositoryInstance = CreateRepositoryInstance<TRepository>();
            _repositories.Add(entityType, repositoryInstance);
        }

        return (TRepository)_repositories[entityType];
    }

    /// <summary>
    /// Salva todas as alterações feitas no contexto do banco de dados de forma assíncrona.
    /// </summary>
    /// <param name="cancellationToken">Um token de cancelamento para observar enquanto espera que a tarefa seja concluída.</param>
    /// <returns>Um objeto Task que representa a operação de salvar as mudanças assincronamente.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _commandContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Libera todos os recursos usados pela instância atual da classe UnitOfWork.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        // Evita a chamada do finalizador pelo GC, pois todos os recursos não gerenciados já foram liberados.
        GC.SuppressFinalize(this);
    }

    private TRepository CreateRepositoryInstance<TRepository>()
    {
        return _serviceProvider.GetRequiredService<TRepository>();
    }

    /// <summary>
    /// Libera os recursos não gerenciados usados pela instância atual da classe UnitOfWork e,
    /// opcionalmente, libera os recursos gerenciados.
    /// </summary>
    /// <param name="disposing">true para liberar os recursos gerenciados e não gerenciados; false para liberar apenas os recursos não gerenciados.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _commandContext.Dispose();
            _queryContext.Dispose();
        }

        _disposed = true;
    }
}