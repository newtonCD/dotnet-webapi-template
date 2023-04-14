using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Template.Domain;

[ExcludeFromCodeCoverage]
public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
