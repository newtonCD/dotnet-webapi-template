using Infrastructure.Extensions;
using System;
using System.Collections.Generic;

namespace Template.Infrastructure.Tests.Extensions;

public class TypeExtensionsTests
{
    [Fact]
    public void IsAssignableToGenericType_ReturnsTrue_ForDerivedGenericType()
    {
        // Arrange
        Type listType = typeof(List<int>);
        Type genericType = typeof(IEnumerable<>);

        // Act
        bool result = listType.IsAssignableToGenericType(genericType);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAssignableToGenericType_ReturnsFalse_ForNonDerivedGenericType()
    {
        // Arrange
        Type customType = typeof(NonTestImplementation);
        Type genericType = typeof(IEnumerable<>);

        // Act
        bool result = customType.IsAssignableToGenericType(genericType);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAssignableToGenericType_ReturnsTrue_ForDerivedGenericInterface()
    {
        // Arrange
        Type testType = typeof(TestImplementation);
        Type genericType = typeof(ITestInterface<>);

        // Act
        bool result = testType.IsAssignableToGenericType(genericType);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAssignableToGenericType_ReturnsFalse_ForNonDerivedGenericInterface()
    {
        // Arrange
        Type testType = typeof(NonTestImplementation);
        Type genericType = typeof(ITestInterface<>);

        // Act
        bool result = testType.IsAssignableToGenericType(genericType);

        // Assert
        Assert.False(result);
    }
}

// Test classes and interfaces for the unit tests
public interface ITestInterface<T> { }

public class TestImplementation : ITestInterface<string> { }

public class NonTestImplementation { }
