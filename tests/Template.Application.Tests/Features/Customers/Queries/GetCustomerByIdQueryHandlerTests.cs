using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Exceptions;
using Template.Application.Common.Interfaces;
using Template.Application.Features.Customers.Queries;
using Template.Application.Settings;
using Template.Domain.Entities;
using Template.Domain.Interfaces;

namespace Template.Application.Tests.Features.Customers.Queries;

public class GetCustomerByIdQueryHandlerTests
{
    private readonly Mock<IBaseRepository<Customer>> _customerRepositoryMock;
    private readonly GetCustomerByIdQueryHandler _handler;
    private readonly ICustomServiceProvider _serviceProvider;
    private readonly CacheSettings _cacheSettings;

    public GetCustomerByIdQueryHandlerTests()
    {
        _customerRepositoryMock = new Mock<IBaseRepository<Customer>>();
        _handler = new GetCustomerByIdQueryHandler(_customerRepositoryMock.Object);

        var serviceProviderMock = new Mock<ICustomServiceProvider>();

        _cacheSettings = new CacheSettings
        {
            ExpirationInMinutes = 5
        };

        serviceProviderMock
            .Setup(x => x.GetService<IOptions<CacheSettings>>())
            .Returns(Options.Create(_cacheSettings));

        _serviceProvider = serviceProviderMock.Object;
    }

    [Fact]
    public void CacheKey_ShouldReturnCorrectValue()
    {
        // Arrange
        int customerId = 1;
        var query = new GetCustomerByIdQuery(customerId);

        // Act
        string cacheKey = query.CacheKey;

        // Assert
        Assert.Equal($"Customer:{customerId}", cacheKey);
    }

    [Fact]
    public void GetCacheExpiration_ShouldReturnCorrectValueFromCacheSettings()
    {
        // Arrange
        int customerId = 1;
        var query = new GetCustomerByIdQuery(customerId);

        // Act
        TimeSpan? cacheExpiration = query.GetCacheExpiration(_serviceProvider);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(_cacheSettings.ExpirationInMinutes), cacheExpiration);
    }

    [Fact]
    public async Task Handle_CustomerFound_ShouldReturnCustomerResponse()
    {
        // Arrange
        int customerId = 1;
        var customer = CreateCustomerWithId(customerId, "John Doe", "john.doe@example.com");
        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        var query = new GetCustomerByIdQuery(customerId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal(customer.Name, result.Name);
        Assert.Equal(customer.Email, result.Email);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        int customerId = 1;
        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync((Customer)null);

        var query = new GetCustomerByIdQuery(customerId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }

    private static Customer CreateCustomerWithId(int id, string name, string email)
    {
        Customer customer = new(name, email);
        typeof(Customer).GetProperty("Id")?.SetValue(customer, id);
        return customer;
    }
}
