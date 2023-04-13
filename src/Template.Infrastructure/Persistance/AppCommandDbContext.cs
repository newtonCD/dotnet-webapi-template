using Microsoft.EntityFrameworkCore;
using Template.Domain.Interfaces;

namespace Template.Infrastructure.Persistance;

public class AppCommandDbContext : AppDbContextBase, IAppCommandDbContext
{
    public AppCommandDbContext(DbContextOptions<AppCommandDbContext> options)
        : base(options)
    {
    }
}
