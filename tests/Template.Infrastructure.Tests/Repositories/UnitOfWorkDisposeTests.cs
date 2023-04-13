using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Template.Domain.Interfaces;
using Template.Infrastructure.Persistance;
using Template.Infrastructure.Repositories;

namespace Template.Infrastructure.Tests.Repositories;

public class UnitOfWorkDisposeTests : IDisposable
{
    private readonly AppCommandDbContext _commandContext;
    private readonly AppQueryDbContext _queryContext;
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkDisposeTests()
    {
        var options = new DbContextOptionsBuilder<AppCommandDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _commandContext = new AppCommandDbContext(options);

        var queryOptions = new DbContextOptionsBuilder<AppQueryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _queryContext = new AppQueryDbContext(queryOptions);

        var serviceProvider = new ServiceCollection()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<AppCommandDbContext>(_ => _commandContext)
            .AddScoped<AppQueryDbContext>(_ => _queryContext)
            .BuildServiceProvider();

        _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
    }

    [Fact]
    public void Dispose_ShouldDisposeContexts()
    {
        // Act
        _unitOfWork.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => _commandContext.Database.EnsureCreated());
        Assert.Throws<ObjectDisposedException>(() => _queryContext.Database.EnsureCreated());
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
    }
}
