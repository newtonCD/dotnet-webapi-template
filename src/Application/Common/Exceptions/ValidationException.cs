using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class ValidationException : CustomException
{
    public ValidationException(string message, IDictionary<string, string[]> errors)
        : base(message, HttpStatusCode.BadRequest)
    {
        Errors = errors;
    }

    public IDictionary<string, string[]> Errors { get; }
}
