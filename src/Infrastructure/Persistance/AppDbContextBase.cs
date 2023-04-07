using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance;

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:Single-line comment should be preceded by blank line", Justification = "<Pending>")]
public abstract class AppDbContextBase : DbContext, IAppDbContextBase
{
    protected AppDbContextBase(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
// TEMPLATE - nao remover ou alterar essa linha

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder?.ApplyConfigurationsFromAssembly(typeof(AppDbContextBase).Assembly);
    }
}
