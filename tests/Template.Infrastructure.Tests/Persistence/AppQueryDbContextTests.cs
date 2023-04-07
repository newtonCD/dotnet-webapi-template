using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Template.Infrastructure.Tests.Persistence;

public class AppQueryDbContextTests
{
    [Fact]
    public void AppQueryDbContext_ShouldBeOfTypeIAppQueryDbContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppQueryDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Act
        using (var context = new TestDbContext(options))
        {
            // Assert
            Assert.IsAssignableFrom<IAppQueryDbContext>(context);
        }
    }

    private class TestDbContext : AppQueryDbContext
    {
        public TestDbContext(DbContextOptions<AppQueryDbContext> options) : base(options)
        {
        }
    }
}
