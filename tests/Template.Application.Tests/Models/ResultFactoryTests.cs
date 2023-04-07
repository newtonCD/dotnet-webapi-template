using Application.Common.Models;
using System.Collections.Generic;

namespace Template.Application.Tests.Models;

public class ResultFactoryTests
{
    [Fact]
    public void Success_ShouldReturnResultWithSucceededTrueAndEmptyErrors()
    {
        // Act
        var result = ResultFactory.Success();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Success_WithGenericType_ShouldReturnResultWithSucceededTrueAndEmptyErrors()
    {
        // Arrange
        int data = 42;

        // Act
        var result = ResultFactory.Success(data);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(data, result.Data);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_ShouldReturnResultWithSucceededFalseAndGivenErrors()
    {
        // Arrange
        var errors = new List<string> { "Error1", "Error2" };

        // Act
        var result = ResultFactory.Failure(errors);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(errors, result.Errors);
    }

    [Fact]
    public void Failure_WithGenericType_ShouldReturnResultWithSucceededFalseAndGivenErrors()
    {
        // Arrange
        var errors = new List<string> { "Error1", "Error2" };

        // Act
        var result = ResultFactory.Failure<int>(errors);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(errors, result.Errors);
        Assert.Equal(default(int), result.Data);
    }
}
