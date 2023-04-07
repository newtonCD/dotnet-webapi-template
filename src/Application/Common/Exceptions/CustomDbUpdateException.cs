using System;

namespace Application.Common.Exceptions;

public class CustomDbUpdateException : Exception
{
    public CustomDbUpdateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}