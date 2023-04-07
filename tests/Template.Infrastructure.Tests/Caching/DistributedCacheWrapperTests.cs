using Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Infrastructure.Tests.Caching;

public class DistributedCacheWrapperTests
{
    private readonly Mock<IDistributedCache> _mockDistributedCache;
    private readonly DistributedCacheWrapper _distributedCacheWrapper;

    public DistributedCacheWrapperTests()
    {
        _mockDistributedCache = new Mock<IDistributedCache>();
        _distributedCacheWrapper = new DistributedCacheWrapper(_mockDistributedCache.Object);
    }

    [Fact]
    public async Task GetStringAsync_ShouldCallUnderlyingCache()
    {
        // Arrange
        var key = "testKey";
        var value = "testValue";
        _mockDistributedCache.Setup(x => x.GetAsync(key, CancellationToken.None)).ReturnsAsync(Encoding.UTF8.GetBytes(value));

        // Act
        var result = await _distributedCacheWrapper.GetStringAsync(key);

        // Assert
        Assert.Equal(value, result);
        _mockDistributedCache.Verify(x => x.GetAsync(key, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SetStringAsync_ShouldCallUnderlyingCache()
    {
        // Arrange
        var key = "testKey";
        var value = "testValue";
        var options = new DistributedCacheEntryOptions();

        _mockDistributedCache.Setup(x => x.SetAsync(key, It.IsAny<byte[]>(), options, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        await _distributedCacheWrapper.SetStringAsync(key, value, options);

        // Assert
        _mockDistributedCache.Verify(x => x.SetAsync(key, It.Is<byte[]>(bytes => Encoding.UTF8.GetString(bytes) == value), options, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallUnderlyingCache()
    {
        // Arrange
        string key = "testKey";
        _mockDistributedCache.Setup(x => x.RemoveAsync(key, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        await _distributedCacheWrapper.RemoveAsync(key);

        // Assert
        _mockDistributedCache.Verify(x => x.RemoveAsync(key, CancellationToken.None), Times.Once);
    }
}