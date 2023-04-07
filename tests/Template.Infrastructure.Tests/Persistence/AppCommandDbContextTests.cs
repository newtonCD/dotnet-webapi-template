using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Template.Infrastructure.Tests.Persistence;

public class AppCommandDbContextTests
{
    [Fact]
    public void AppCommandDbContext_ShouldBeOfTypeIAppCommandDbContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppCommandDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Act
        using (var context = new TestDbContext(options))
        {
            // Assert
            Assert.IsAssignableFrom<IAppCommandDbContext>(context);
        }
    }

    private class TestDbContext : AppCommandDbContext
    {
        public TestDbContext(DbContextOptions<AppCommandDbContext> options) : base(options)
        {
        }
    }
}
