using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Template.WebApi;

[ExcludeFromCodeCoverage]
public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}