using Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Services;

[ExcludeFromCodeCoverage]
public class CustomServiceProvider : ICustomServiceProvider
{
    private readonly IServiceProvider _serviceProvider;

    public CustomServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T GetService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}