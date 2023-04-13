using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities;
using Template.Infrastructure.Persistance;

namespace Template.Infrastructure.Tests.Persistence;

public class AppDbContextBaseTests
{
    [Fact]
    public void AppDbContextBase_ShouldCreateCustomersTable()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContextBase>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Act
        using (var context = new TestDbContext(options))
        {
            context.Database.EnsureCreated();
            var exists = context.Model.FindEntityType(typeof(Customer)) != null;

            // Assert
            Assert.True(exists);
        }
    }

    private class TestDbContext : AppDbContextBase
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
