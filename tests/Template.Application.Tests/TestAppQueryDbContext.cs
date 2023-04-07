using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
