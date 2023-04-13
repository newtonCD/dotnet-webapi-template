using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Template.Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class InternalServerException : CustomException
{
    public InternalServerException(string message)
        : base(message, HttpStatusCode.InternalServerError)
    {
    }
}