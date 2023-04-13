using Microsoft.EntityFrameworkCore;
using Template.Domain.Interfaces;

namespace Template.Infrastructure.Persistance;

public class AppQueryDbContext : AppDbContextBase, IAppQueryDbContext
{
    public AppQueryDbContext(DbContextOptions<AppQueryDbContext> options)
        : base(options)
    {
    }
}