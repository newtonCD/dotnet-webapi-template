using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Template.WebApi.Filters;
using Template.WebApi.Presenters;

namespace Template.WebApi.Controllers;

/// <summary>
/// Representa a API base para as outras controllers.
/// </summary>
[ApiController]
[ApiExceptionHandlingFilter]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status500InternalServerError)]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _sender;

    protected ApiControllerBase()
    {
    }

    protected ApiControllerBase(ISender sender)
    {
        _sender = sender;
    }

    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
}
