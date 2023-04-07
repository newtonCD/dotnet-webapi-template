using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class NotFoundException : CustomException
{
    public NotFoundException(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }
}