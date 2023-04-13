using System.Reflection;

namespace Template.Infrastructure;

public static class AssemblyEntryPoint
{
    public static readonly Assembly Assembly = typeof(AssemblyEntryPoint).Assembly;
}