using Domain.Entities;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Template.Infrastructure.Tests.Repositories;

public class BaseRepositoryTests
{
    private readonly AppCommandDbContext _commandContext;
    private readonly AppQueryDbContext _queryContext;
    private readonly BaseRepository<Customer> _repository;

    public BaseRepositoryTests()
    {
        var dbName = Guid.NewGuid().ToString();

        var databaseRoot = new InMemoryDatabaseRoot();

        var options = new DbContextOptionsBuilder<AppCommandDbContext>()
              .UseInMemoryDatabase(dbName, databaseRoot)
              .Options;
        _commandContext = new AppCommandDbContext(options);

        var queryOptions = new DbContextOptionsBuilder<AppQueryDbContext>()
            .UseInMemoryDatabase(dbName, databaseRoot)
            .Options;
        _queryContext = new AppQueryDbContext(queryOptions);

        _repository = new CustomerRepository(_commandContext, _queryContext);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityToCommandContext()
    {
        // Arrange
        var customer = new Customer("John Doe", "johndoe@email.com");

        // Act
        await _repository.AddAsync(customer);
        await _commandContext.SaveChangesAsync();

        // Assert
        var result = await _commandContext.Customers.FindAsync(customer.Id);
        Assert.NotNull(result);
        Assert.Equal(customer.Name, result.Name);
    }

    [Fact]
    public async Task Delete_ShouldRemoveEntityFromCommandContext()
    {
        // Arrange
        var customer = new Customer("John Doe", "johndoe@email.com");
        await _repository.AddAsync(customer);
        await _commandContext.SaveChangesAsync();

        // Act
        _repository.Delete(customer);
        await _commandContext.SaveChangesAsync();

        // Assert
        var result = await _commandContext.Customers.FindAsync(customer.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var customer1 = new Customer("John Doe", "johndoe@email.com");
        var customer2 = new Customer("Jane Doe", "janedoe@email.com");

        await _repository.AddAsync(customer1);
        await _repository.AddAsync(customer2);
        await _commandContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntityWithGivenId()
    {
        // Arrange
        var customer = new Customer("John Doe", "johndoe@email.com");

        await _repository.AddAsync(customer);
        await _commandContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(customer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueIfEntityExists()
    {
        // Arrange
        var customer = new Customer("John Doe", "johndoe@email.com");

        await _repository.AddAsync(customer);
        await _commandContext.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(customer.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnEntitiesMatchingPredicate()
    {
        // Arrange
        var customer1 = new Customer("John Doe", "johndoe@email.com");
        var customer2 = new Customer("Jane Doe", "janedoe@email.com");

        await _repository.AddAsync(customer1);
        await _repository.AddAsync(customer2);
        await _commandContext.SaveChangesAsync();

        Expression<Func<Customer, bool>> predicate = c => c.Name.Contains("Doe");

        // Act
        var result = await _repository.FindAsync(predicate);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CountAsync_ShouldReturnNumberOfEntities()
    {
        // Arrange
        var customer1 = new Customer("John Doe", "johndoe@email.com");
        var customer2 = new Customer("Jane Doe", "janedoe@email.com");

        await _repository.AddAsync(customer1);
        await _repository.AddAsync(customer2);
        await _commandContext.SaveChangesAsync();

        // Act
        var count = await _repository.CountAsync();

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntityInCommandContext()
    {
        // Arrange
        var customer = new Customer("John Doe", "johndoe@email.com");
        await _repository.AddAsync(customer);
        await _commandContext.SaveChangesAsync();

        // Act
        customer.Update("Johnny Doe", "johndoe_updated@email.com");
        _repository.Update(customer);
        await _commandContext.SaveChangesAsync();

        // Assert
        var result = await _commandContext.Customers.FindAsync(customer.Id);
        Assert.NotNull(result);
        Assert.Equal("Johnny Doe", result.Name);
        Assert.Equal("johndoe_updated@email.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullForNonExistingEntity()
    {
        // Arrange
        var nonExistingId = -999999999;

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalseForNonExistingEntity()
    {
        // Arrange
        var nonExistingId = 1;

        // Act
        var exists = await _repository.ExistsAsync(nonExistingId);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnEntitiesInRequestedPage()
    {
        // Arrange
        var customer1 = new Customer("John Doe", "johndoe@email.com");
        var customer2 = new Customer("Jane Doe", "janedoe@email.com");
        var customer3 = new Customer("Jimmy Doe", "jimmydoe@email.com");
        var customer4 = new Customer("Jenny Doe", "jennydoe@email.com");

        await _repository.AddAsync(customer1);
        await _repository.AddAsync(customer2);
        await _repository.AddAsync(customer3);
        await _repository.AddAsync(customer4);
        await _commandContext.SaveChangesAsync();

        int pageNumber = 2;
        int pageSize = 2;

        // Act
        var result = await _repository.GetPagedAsync(pageNumber, pageSize);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == customer3.Id);
        Assert.Contains(result, c => c.Id == customer4.Id);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnEmptyList_WhenRequestedPageIsEmpty()
    {
        // Arrange
        var customer1 = new Customer("John Doe", "johndoe@email.com");
        var customer2 = new Customer("Jane Doe", "janedoe@email.com");

        await _repository.AddAsync(customer1);
        await _repository.AddAsync(customer2);
        await _commandContext.SaveChangesAsync();

        int pageNumber = 3;
        int pageSize = 2;

        // Act
        var result = await _repository.GetPagedAsync(pageNumber, pageSize);

        // Assert
        Assert.Empty(result);
    }

    [Fact(Skip = "Ainda falta implementar o controle de duplicidade.")]
    public async Task AddAsync_ShouldThrowExceptionForDuplicateEntity()
    {
        // Arrange
        var customer = new Customer("John Doe", "johndoe@email.com");
        await _repository.AddAsync(customer);
        await _commandContext.SaveChangesAsync();

        var duplicateCustomer = new Customer("John Doe", "johndoe@email.com");

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(duplicateCustomer));
    }
}
