using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Infrastructure.Tests.Repositories;

public class UnitOfWorkTests : IDisposable
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppQueryDbContext _queryContext;
    private readonly IAppCommandDbContext _commandContext;

    public UnitOfWorkTests()
    {
        var services = new ServiceCollection();

        // In-memory database setup
        services.AddDbContext<AppQueryDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDbQuery");
        });
        services.AddDbContext<AppCommandDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDbCommand");
        });

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var serviceProvider = services.BuildServiceProvider();

        _queryContext = serviceProvider.GetRequiredService<AppQueryDbContext>();
        _commandContext = serviceProvider.GetRequiredService<AppCommandDbContext>();
        _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSaveChangesToDatabase()
    {
        // Arrange
        var customer = new Customer("John Doe", "teste@email.com");
        await _unitOfWork.GetRepository<ICustomerRepository, Customer>().AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        // Act
        var result = await _commandContext.Customers.FindAsync(customer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Name, result.Name);
    }

    [Fact]
    public void GetRepository_ShouldReturnRepositoryInstance()
    {
        // Act
        var result = _unitOfWork.GetRepository<ICustomerRepository, Customer>();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CustomerRepository>(result);
    }

    public void Dispose()
    {
        _commandContext.Dispose();
        _queryContext.Dispose();
    }
}