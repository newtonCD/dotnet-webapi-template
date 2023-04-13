using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Template.Application.Common.Models;

public class Result<T> : Result
{
    [JsonConstructor]
    public Result()
        : base(true, new List<string>())
    {
    }

    internal Result(bool succeeded, T data, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Data = data;
    }

    public T Data { get; init; }
}