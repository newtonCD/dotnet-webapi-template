
using Template.Shared.Extensions;

namespace Template.Application.Tests.Extensions;

public class ObjectExtensionsTests
{
    private class SimpleObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class ComplexObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SimpleObject NestedObject { get; set; }
    }

    [Fact]
    public void DeepCopy_ShouldCreateDeepCopyOfSimpleObject()
    {
        // Arrange
        var originalObject = new SimpleObject { Id = 1, Name = "Test" };

        // Act
        var deepCopy = originalObject.DeepCopy();

        // Assert
        Assert.NotSame(originalObject, deepCopy);
        Assert.Equal(originalObject.Id, deepCopy.Id);
        Assert.Equal(originalObject.Name, deepCopy.Name);
    }

    [Fact]
    public void DeepCopy_ShouldCreateDeepCopyOfComplexObject()
    {
        // Arrange
        var originalObject = new ComplexObject
        {
            Id = 1,
            Name = "Test",
            NestedObject = new SimpleObject { Id = 2, Name = "Nested Test" }
        };

        // Act
        var deepCopy = originalObject.DeepCopy();

        // Assert
        Assert.NotSame(originalObject, deepCopy);
        Assert.Equal(originalObject.Id, deepCopy.Id);
        Assert.Equal(originalObject.Name, deepCopy.Name);
        Assert.NotSame(originalObject.NestedObject, deepCopy.NestedObject);
        Assert.Equal(originalObject.NestedObject.Id, deepCopy.NestedObject.Id);
        Assert.Equal(originalObject.NestedObject.Name, deepCopy.NestedObject.Name);
    }

    [Fact]
    public void DeepCopy_ShouldReturnDefaultWhenSourceIsNull()
    {
        // Arrange
        SimpleObject originalObject = null;

        // Act
        var deepCopy = originalObject.DeepCopy();

        // Assert
        Assert.Null(deepCopy);
    }
}
