using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Domain.Tests.Common;

public class BaseAuditableEntityTests
{
    private class TestAuditableEntity : BaseAuditableEntity
    {
    }

    [Fact]
    public void BaseAuditableEntity_DefaultConstructor_SetsCreated()
    {
        // Act
        var testEntity = new TestAuditableEntity();

        // Assert
        Assert.NotEqual(DateTime.MinValue, testEntity.Created);
    }
}
