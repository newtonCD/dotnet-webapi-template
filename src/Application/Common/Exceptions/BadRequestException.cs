using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class BadRequestException : CustomException
{
    public BadRequestException(string message)
        : base(message, HttpStatusCode.BadRequest)
    {
    }
}