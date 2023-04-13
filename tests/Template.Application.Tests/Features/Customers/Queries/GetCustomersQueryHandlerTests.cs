using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Interfaces;
using Template.Application.Features.Customers.Queries;
using Template.Application.Settings;
using Template.Domain.Entities;
using Template.Domain.Interfaces;

namespace Template.Application.Tests.Features.Customers.Queries;

public class GetCustomersQueryHandlerTests
{
    private readonly AutoMocker _mocker;
    private readonly GetCustomersQueryHandler _handler;

    public GetCustomersQueryHandlerTests()
    {
        _mocker = new AutoMocker();
        _handler = _mocker.CreateInstance<GetCustomersQueryHandler>();
    }

    [Fact]
    public void CacheKey_ShouldBeGeneratedCorrectly()
    {
        // Arrange
        int pageNumber = 1;
        int pageSize = 10;

        // Act
        var query = new GetCustomersQuery(pageNumber, pageSize);

        // Assert
        query.CacheKey.Should().Be($"Customer:Paging:{pageNumber}_{pageSize}");
    }

    [Fact]
    public void GetCacheExpiration_ShouldReturnCorrectValueFromCacheSettings()
    {
        // Arrange
        int pageNumber = 1;
        int pageSize = 10;
        int expirationInMinutes = 5;
        var query = new GetCustomersQuery(pageNumber, pageSize);

        var serviceProviderMock = new Mock<ICustomServiceProvider>();
        var optionsMock = new Mock<IOptions<CacheSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new CacheSettings { ExpirationInMinutes = expirationInMinutes });
        serviceProviderMock.Setup(s => s.GetService<IOptions<CacheSettings>>()).Returns(optionsMock.Object);

        // Act
        TimeSpan? cacheExpiration = query.GetCacheExpiration(serviceProviderMock.Object);

        // Assert
        cacheExpiration.Should().Be(TimeSpan.FromMinutes(expirationInMinutes));
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedCustomerResponse()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var query = new GetCustomersQuery(pageNumber, pageSize);

        var customers = new List<Customer>
        {
            new Customer("John Doe", "john.doe@example.com"),
            new Customer("Jane Doe", "jane.doe@example.com")
        };

        var repositoryMock = _mocker.GetMock<IBaseRepository<Customer>>();
        repositoryMock.Setup(r => r.CountAsync())
            .ReturnsAsync(customers.Count);
        repositoryMock.Setup(r => r.GetPagedAsync(pageNumber, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalPages.Should().Be(1);
        result.TotalItems.Should().Be(customers.Count);
        result.Items.Should().HaveCount(customers.Count);

        var expectedCustomerSummaryResponses = customers.Select(c => new CustomerSummaryResponse(c.Id, c.Name, c.Email)).ToList();
        result.Items.Should().BeEquivalentTo(expectedCustomerSummaryResponses, options => options.WithStrictOrdering());
    }
}