using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class ForbiddenAccessException : CustomException
{
    public ForbiddenAccessException(string message)
        : base(message, HttpStatusCode.Forbidden)
    {
    }
}
