using System.Collections.Generic;

namespace Application.Common.Models;

public class Result<T> : Result
{
    internal Result(bool succeeded, T data, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Data = data;
    }

    public T Data { get; init; }
}