using System;
using System.Collections.Generic;

namespace Application.Common.Models;

public static class ResultFactory
{
    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result<T> Success<T>(T data)
    {
        return new Result<T>(true, data, Array.Empty<string>());
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0034:Simplify 'default' expression", Justification = "<Pending>")]
    public static Result<T> Failure<T>(IEnumerable<string> errors)
    {
        return new Result<T>(false, default(T), errors);
    }
}
