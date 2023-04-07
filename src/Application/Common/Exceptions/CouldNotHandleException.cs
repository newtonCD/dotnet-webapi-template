using System;
using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class CouldNotHandleException : Exception
{
    public CouldNotHandleException()
    {
    }

    public CouldNotHandleException(string message)
        : base(message)
    {
    }

    public CouldNotHandleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}