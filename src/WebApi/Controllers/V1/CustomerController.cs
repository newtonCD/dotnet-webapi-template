using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Features.Customers.Commands;
using Application.Features.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Throw;
using WebApi.Presenters;

namespace WebApi.Controllers.V1;

public sealed class CustomerController : ApiControllerBase
{
    public CustomerController(ISender sender)
        : base(sender)
    {
    }

    /// <summary>
    /// Obter um cliente pelo seu ID.
    /// </summary>
    /// <param name="customerId">O ID do cliente a ser obtido.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>O cliente com o ID especificado.</returns>
    [HttpGet("{customerId:int}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int customerId, CancellationToken cancellationToken)
    {
        CustomerResponse customer = await Sender.Send(new GetCustomerByIdQuery(customerId), cancellationToken).ConfigureAwait(false);

        return Ok(customer);
    }

    /// <summary>
    /// Cadastrar um novo cliente.
    /// </summary>
    /// <param name="command">Dados do cliente a ser criado.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>O ID do cliente criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        Result<int> result = await Sender.Send(command, cancellationToken).ConfigureAwait(false);

        return result.Succeeded
            ? CreatedAtAction(nameof(Create), new { customerId = result.Data }, result)
            : UnprocessableEntity(new CustomProblemDetails(result.Errors));
    }

    /// <summary>
    /// Deletar um cliente existente.
    /// </summary>
    /// <param name="customerId">O ID do cliente a ser deletado.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmação de que o cliente foi deletado com sucesso.</returns>
    [HttpDelete("{customerId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int customerId, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteCustomerCommand(customerId), cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Atualizar um cliente existente.
    /// </summary>
    /// <param name="command">Um objeto json com os dados atualizados do cliente.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmação de que o cliente foi atualizado com sucesso.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> Update([FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        await Sender.Send(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Obtém todos os clientes com paginação.
    /// </summary>
    /// <param name="pageNumber">O número da página a ser recuperado (inicia em 1).</param>
    /// <param name="pageSize">A quantidade de itens por página (padrão é 10).</param>
    /// <returns>Uma ActionResult contendo um objeto PagedCustomerResponse com a lista paginada de clientes e informações de paginação.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedCustomerResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedCustomerResponse>> GetAllCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        GetCustomersQuery query = new GetCustomersQuery(pageNumber, pageSize);
        PagedCustomerResponse result = await Sender.Send(query).ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>
    /// Obtém todos os clientes com base no endereço de e-mail fornecido.
    /// </summary>
    /// <param name="email">O endereço de e-mail a ser usado como critério de pesquisa.</param>
    /// <returns>Uma ActionResult contendo uma lista de objetos CustomerSummaryResponse com os clientes que correspondem ao endereço de e-mail fornecido.</returns>
    [HttpGet("search/by-email")]
    [ProducesResponseType(typeof(IEnumerable<CustomerSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CustomerSummaryResponse>>> GetCustomersByEmail(string email)
    {
        email.ThrowIfNull(() => throw new NotFoundException("O e-mail não foi informado."));

        GetCustomersByEmailQuery query = new GetCustomersByEmailQuery(email);
        IEnumerable<CustomerSummaryResponse> customers = await Sender.Send(query).ConfigureAwait(false);

        return Ok(customers);
    }
}