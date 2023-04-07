using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = new ReadOnlyCollection<string>(errors.ToList());
    }

    public bool Succeeded { get; init; }

    public IReadOnlyCollection<string> Errors { get; init; }
}