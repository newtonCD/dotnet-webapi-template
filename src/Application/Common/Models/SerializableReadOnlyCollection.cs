using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Models;

[ExcludeFromCodeCoverage]
public class SerializableReadOnlyCollection<T> : ReadOnlyCollection<T>
{
    public SerializableReadOnlyCollection(IList<T> list)
        : base(list)
    {
    }
}