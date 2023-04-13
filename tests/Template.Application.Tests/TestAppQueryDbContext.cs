using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities;
using Template.Domain.Interfaces;

namespace Template.Application.Tests;

public class TestAppQueryDbContext : DbContext, IAppQueryDbContext
{
    public TestAppQueryDbContext(DbContextOptions<TestAppQueryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
// TEMPLATE - nao remover ou alterar essa linha

}
