using Microsoft.Extensions.Options;
using Moq;
using System;
using Template.Application.Common.Cache;
using Template.Application.Common.Interfaces;
using Template.Application.Settings;

namespace Template.Application.Tests.Cache;

public class CacheableQueryBaseTests
{
    private class TestCacheableQuery : CacheableQueryBase
    {
        public override string CacheKey => "TestCacheKey";
    }

    [Fact]
    public void GetCacheExpiration_ShouldReturnCorrectExpiration()
    {
        // Arrange
        var cacheSettings = new CacheSettings { ExpirationInMinutes = 5 };
        var optionsMock = new Mock<IOptions<CacheSettings>>();
        optionsMock.Setup(o => o.Value).Returns(cacheSettings);

        var serviceProviderMock = new Mock<ICustomServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService<IOptions<CacheSettings>>()).Returns(optionsMock.Object);

        var testCacheableQuery = new TestCacheableQuery();

        // Act
        TimeSpan? cacheExpiration = testCacheableQuery.GetCacheExpiration(serviceProviderMock.Object);

        // Assert
        Assert.NotNull(cacheExpiration);
        Assert.Equal(TimeSpan.FromMinutes(cacheSettings.ExpirationInMinutes), cacheExpiration);
    }

    [Fact]
    public void GetCacheExpiration_NullServiceProvider_ShouldReturnDefaultExpiration()
    {
        // Arrange
        var testCacheableQuery = new TestCacheableQuery();

        // Act
        TimeSpan? cacheExpiration = testCacheableQuery.GetCacheExpiration(null);

        // Assert
        Assert.Null(cacheExpiration);
    }
}
