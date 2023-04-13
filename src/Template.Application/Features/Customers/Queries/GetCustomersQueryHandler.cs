using System.Collections.ObjectModel;
using System.Linq;
using Template.Application.Features.Base;
using Template.Domain.Entities;
using Template.Domain.Interfaces;

namespace Template.Application.Features.Customers.Queries;

/// <summary>
/// Classe responsável por tratar a consulta para obter todos os clientes.
/// </summary>
public sealed class GetCustomersQueryHandler : PagedQueryHandlerBase<GetCustomersQuery, Customer, CustomerSummaryResponse, PagedCustomerResponse>
{
    public GetCustomersQueryHandler(IBaseRepository<Customer> customerRepository)
        : base(customerRepository)
    {
    }

    /// <summary>
    /// Retorna o número da página da consulta.
    /// </summary>
    /// <param name="request">A consulta para obter todos os clientes.</param>
    /// <returns>O número da página.</returns>
    protected override int GetPageNumber(GetCustomersQuery request)
    {
        return request.PageNumber;
    }

    /// <summary>
    /// Retorna o tamanho da página da consulta.
    /// </summary>
    /// <param name="request">A consulta para obter todos os clientes.</param>
    /// <returns>O tamanho da página.</returns>
    protected override int GetPageSize(GetCustomersQuery request)
    {
        return request.PageSize;
    }

    /// <summary>
    /// Cria a resposta paginada para a consulta.
    /// </summary>
    /// <param name="pageNumber">O número da página.</param>
    /// <param name="pageSize">O tamanho da página.</param>
    /// <param name="totalPages">O número total de páginas.</param>
    /// <param name="totalItems">O número total de itens.</param>
    /// <param name="items">A coleção de itens resumidos dos clientes.</param>
    /// <returns>A resposta paginada de clientes.</returns>
    protected override PagedCustomerResponse CreatePagedResponse(int pageNumber, int pageSize, int totalPages, int totalItems, ReadOnlyCollection<CustomerSummaryResponse> items)
    {
        return new PagedCustomerResponse(pageNumber, pageSize, totalPages, totalItems, new Collection<CustomerSummaryResponse>(items.ToList()));
    }
}