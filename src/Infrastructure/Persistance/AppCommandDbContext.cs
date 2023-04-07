using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance;

public class AppCommandDbContext : AppDbContextBase, IAppCommandDbContext
{
    public AppCommandDbContext(DbContextOptions<AppCommandDbContext> options)
        : base(options)
    {
    }
}
