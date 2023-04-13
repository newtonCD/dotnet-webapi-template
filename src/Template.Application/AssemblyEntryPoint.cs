using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Template.Application;

[ExcludeFromCodeCoverage]
public static class AssemblyEntryPoint
{
    public static readonly Assembly Assembly = typeof(AssemblyEntryPoint).Assembly;
}
