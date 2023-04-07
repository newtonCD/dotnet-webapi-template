using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance;

public class AppQueryDbContext : AppDbContextBase, IAppQueryDbContext
{
    public AppQueryDbContext(DbContextOptions<AppQueryDbContext> options)
        : base(options)
    {
    }
}