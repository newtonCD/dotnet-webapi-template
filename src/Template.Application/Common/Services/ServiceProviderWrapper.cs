using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using Template.Application.Common.Interfaces;

namespace Template.Application.Common.Services;

[ExcludeFromCodeCoverage]
public class ServiceProviderWrapper : ICustomServiceProvider
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceProviderWrapper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T GetService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}
