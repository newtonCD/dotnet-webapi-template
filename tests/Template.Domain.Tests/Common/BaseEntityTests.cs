using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Domain.Tests.Common;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
        public TestEntity() : base() { }
        public TestEntity(int id) : base(id) { }
    }

    private class TestEvent : BaseEvent
    {
    }

    [Fact]
    public void BaseEntity_Constructor_SetsId()
    {
        // Arrange
        int expectedId = 1;

        // Act
        var testEntity = new TestEntity(expectedId);

        // Assert
        Assert.Equal(expectedId, testEntity.Id);
    }
}