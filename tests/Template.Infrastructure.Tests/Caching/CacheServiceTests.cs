using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Interfaces;
using Template.Infrastructure.Caching;

namespace Template.Infrastructure.Tests.Caching;

public class CacheServiceTests
{
    private readonly Mock<IDistributedCacheWrapper> _distributedCacheWrapperMock;
    private readonly CacheService _cacheService;

    public CacheServiceTests()
    {
        _distributedCacheWrapperMock = new Mock<IDistributedCacheWrapper>();
        _cacheService = new CacheService(_distributedCacheWrapperMock.Object);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDeserializedValue_WhenValueExists()
    {
        // Arrange
        string key = "testKey";
        string testValueJson = JsonSerializer.Serialize("testValue");

        _distributedCacheWrapperMock
            .Setup(x => x.GetStringAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testValueJson);

        // Act
        string result = await _cacheService.GetAsync<string>(key);

        // Assert
        Assert.Equal("testValue", result);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDefault_WhenValueDoesNotExist()
    {
        // Arrange
        string key = "nonexistentKey";

        _distributedCacheWrapperMock
            .Setup(x => x.GetStringAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null);

        // Act
        string result = await _cacheService.GetAsync<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_ShouldCallCacheWithStringValueAndOptions()
    {
        // Arrange
        string key = "testKey";
        string value = "testValue";
        TimeSpan expiresIn = TimeSpan.FromMinutes(5);

        // Act
        await _cacheService.SetAsync(key, value, expiresIn);

        // Assert
        _distributedCacheWrapperMock.Verify(x => x.SetStringAsync(
            key,
            It.IsAny<string>(),
            It.Is<DistributedCacheEntryOptions>(options => options.AbsoluteExpirationRelativeToNow == expiresIn),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallCacheToRemoveValue()
    {
        // Arrange
        string key = "testKey";

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert
        _distributedCacheWrapperMock.Verify(x => x.RemoveAsync(
            key,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
