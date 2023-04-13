using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Template.WebApi;

[ExcludeFromCodeCoverage]
public static class AssemblyEntryPoint
{
    public static readonly Assembly Assembly = typeof(AssemblyEntryPoint).Assembly;
}