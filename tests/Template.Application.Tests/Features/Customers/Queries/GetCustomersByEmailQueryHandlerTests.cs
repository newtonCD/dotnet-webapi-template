using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Interfaces;
using Template.Application.Features.Customers.Queries;
using Template.Application.Settings;
using Template.Domain.Entities;
using Template.Domain.Interfaces;

namespace Template.Application.Tests.Features.Customers.Queries;

public class GetCustomersByEmailQueryHandlerTests
{
    private readonly Mock<IBaseRepository<Customer>> _mockCustomerRepository;
    private readonly GetCustomersByEmailQueryHandler _handler;
    private readonly ICustomServiceProvider _serviceProvider;
    private readonly CacheSettings _cacheSettings;

    public GetCustomersByEmailQueryHandlerTests()
    {
        _mockCustomerRepository = new Mock<IBaseRepository<Customer>>();
        _handler = new GetCustomersByEmailQueryHandler(_mockCustomerRepository.Object);
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
        string email = "test@example.com";
        var query = new GetCustomersByEmailQuery(email);

        // Act
        string cacheKey = query.CacheKey;

        // Assert
        Assert.Equal($"Customer:Email:{email}", cacheKey);
    }

    [Fact]
    public void GetCacheExpiration_ShouldReturnCorrectValueFromCacheSettings()
    {
        // Arrange
        string email = "test@example.com";
        var query = new GetCustomersByEmailQuery(email);

        // Act
        TimeSpan? cacheExpiration = query.GetCacheExpiration(_serviceProvider);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(_cacheSettings.ExpirationInMinutes), cacheExpiration);
    }

    [Fact]
    public async Task Handle_ShouldReturnCustomers_WhenEmailExists()
    {
        // Arrange
        string email = "test@example.com";
        var customers = new List<Customer>
            {
                new Customer( "John Doe", email),
                new Customer( "Jane Doe", email)
            };

        _mockCustomerRepository.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()))
            .ReturnsAsync(customers);

        // Act
        var result = await _handler.Handle(new GetCustomersByEmailQuery(email), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenEmailDoesNotExist()
    {
        // Arrange
        string email = "nonexistent@example.com";
        _mockCustomerRepository.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()))
            .ReturnsAsync(new List<Customer>());

        // Act
        var result = await _handler.Handle(new GetCustomersByEmailQuery(email), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}