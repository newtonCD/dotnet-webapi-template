using Domain.Common;
using Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Base;

/// <summary>
/// Classe base para manipulação de consultas paginadas.
/// </summary>
/// <typeparam name="TQuery">O tipo da consulta solicitada.</typeparam>
/// <typeparam name="TEntity">O tipo da entidade.</typeparam>
/// <typeparam name="TSummaryResponse">O tipo do objeto de resposta resumida.</typeparam>
/// <typeparam name="TPagedResponse">O tipo do objeto de resposta paginada.</typeparam>
[ExcludeFromCodeCoverage]
public abstract class PagedQueryHandlerBase<TQuery, TEntity, TSummaryResponse, TPagedResponse> : IRequestHandler<TQuery, TPagedResponse>
    where TQuery : IRequest<TPagedResponse>
    where TEntity : BaseEntity
{
    private readonly IBaseRepository<TEntity> _repository;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="PagedQueryHandlerBase{TQuery, TEntity, TSummaryResponse, TPagedResponse}"/>.
    /// </summary>
    /// <param name="repository">O repositório da entidade.</param>
    protected PagedQueryHandlerBase(IBaseRepository<TEntity> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Manipula a solicitação de consulta paginada.
    /// </summary>
    /// <param name="request">A consulta solicitada.</param>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>A resposta paginada.</returns>
    public async Task<TPagedResponse> Handle(TQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = GetPageNumber(request);
        int pageSize = GetPageSize(request);

        int totalItems = await _repository.CountAsync().ConfigureAwait(false);
        List<TEntity> entities = await _repository.GetPagedAsync(pageNumber, pageSize, cancellationToken).ConfigureAwait(false);
        List<TSummaryResponse> items = entities.Adapt<List<TSummaryResponse>>();
        ReadOnlyCollection<TSummaryResponse> readOnlyItems = items.AsReadOnly();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return CreatePagedResponse(pageNumber, pageSize, totalPages, totalItems, readOnlyItems);
    }

    /// <summary>
    /// Obtém o número da página a partir da consulta solicitada.
    /// </summary>
    /// <param name="request">A consulta solicitada.</param>
    /// <returns>O número da página.</returns>
    protected abstract int GetPageNumber(TQuery request);

    /// <summary>
    /// Obtém a quantidade de registros da página a partir da consulta solicitada.
    /// </summary>
    /// <param name="request">A consulta solicitada.</param>
    /// <returns>A quantidade de registros da página.</returns>
    protected abstract int GetPageSize(TQuery request);

    /// <summary>
    /// Cria uma resposta paginada com os dados fornecidos.
    /// </summary>
    /// <param name="pageNumber">Número da página atual.</param>
    /// <param name="pageSize">Quantidade de itens por página.</param>
    /// <param name="totalPages">Total de páginas.</param>
    /// <param name="totalItems">Total de itens.</param>
    /// <param name="items">Coleção somente leitura dos itens do tipo TSummaryResponse.</param>
    /// <returns>Retorna um objeto TPagedResponse com os dados da resposta paginada.</returns>
    protected abstract TPagedResponse CreatePagedResponse(int pageNumber, int pageSize, int totalPages, int totalItems, ReadOnlyCollection<TSummaryResponse> items);
}