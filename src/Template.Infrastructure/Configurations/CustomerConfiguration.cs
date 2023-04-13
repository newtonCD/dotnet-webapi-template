using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Domain.Entities;

namespace Template.Infrastructure.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    /// <summary>
    /// Este método é chamado automaticamente pelo Entity Framework Core quando se utiliza a função
    /// modelBuilder.ApplyConfigurationsFromAssembly no método OnModelCreating da classe de contexto.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder?.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(100);
    }
}