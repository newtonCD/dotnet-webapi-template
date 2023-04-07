namespace Application.Features.Base;

/// <summary>
/// Interface para consulta genérica de entidades.
/// </summary>
public interface IGetPagingQuery
{
    /// <summary>
    /// Número da página atual.
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// Quantidade de itens por página.
    /// </summary>
    int PageSize { get; }
}