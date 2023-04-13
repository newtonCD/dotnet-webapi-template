using Microsoft.EntityFrameworkCore;
using Template.Domain.Interfaces;
using Template.Infrastructure.Persistance;

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
