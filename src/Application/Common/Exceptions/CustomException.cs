#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public abstract class CustomException : Exception
{
    protected CustomException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}