using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Extensions;

/// <summary>
/// Essa classe de extensão pode ser útil em cenários onde você precisa verificar se um tipo pode
/// ser atribuído a um tipo genérico, por exemplo, ao trabalhar com injeção de dependência ou reflexão
/// para descobrir e registrar automaticamente serviços em um container IoC.
/// </summary>
[ExcludeFromCodeCoverage]
public static class TypeExtensions
{
    /// <summary>
    /// Este método é útil quando você deseja verificar se um determinado tipo (givenType) é atribuível
    /// a um tipo genérico (genericType).
    /// </summary>
    /// <param name="givenType"></param>
    /// <param name="genericType"></param>
    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        Type[] interfaceTypes = givenType.GetInterfaces();

        foreach (Type it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        Type baseType = givenType.BaseType;
        if (baseType == null) return false;

        return IsAssignableToGenericType(baseType, genericType);
    }
}
