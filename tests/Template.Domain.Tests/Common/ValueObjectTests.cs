using System.Collections.Generic;
using Template.Domain.Common;

namespace Template.Domain.Tests.Common;

public class ValueObjectTests
{
    private class TestValueObject : ValueObject
    {
        public int Id { get; }
        public string Name { get; }

        public TestValueObject(int id, string name)
        {
            Id = id;
            Name = name;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return Name;
        }
    }

    [Fact]
    public void Equals_SameProperties_ShouldReturnTrue()
    {
        // Arrange
        var obj1 = new TestValueObject(1, "Test");
        var obj2 = new TestValueObject(1, "Test");

        // Act
        bool isEqual = obj1.Equals(obj2);

        // Assert
        Assert.True(isEqual);
    }

    [Fact]
    public void Equals_DifferentProperties_ShouldReturnFalse()
    {
        // Arrange
        var obj1 = new TestValueObject(1, "Test");
        var obj2 = new TestValueObject(2, "Test");

        // Act
        bool isEqual = obj1.Equals(obj2);

        // Assert
        Assert.False(isEqual);
    }

    [Fact]
    public void GetHashCode_SameProperties_ShouldReturnSameHashCode()
    {
        // Arrange
        var obj1 = new TestValueObject(1, "Test");
        var obj2 = new TestValueObject(1, "Test");

        // Act
        int hash1 = obj1.GetHashCode();
        int hash2 = obj2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_DifferentProperties_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var obj1 = new TestValueObject(1, "Test");
        var obj2 = new TestValueObject(2, "Test");

        // Act
        int hash1 = obj1.GetHashCode();
        int hash2 = obj2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }
}
