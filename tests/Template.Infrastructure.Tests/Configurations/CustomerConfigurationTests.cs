using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Infrastructure.Tests.Configurations;

public class CustomerConfigurationTests
{
    [Fact]
    public void Configure_Should_Set_Customer_Id_As_Key()
    {
        // Arrange
        var builder = new EntityTypeBuilder<Customer>(new ModelBuilder(new DbContextOptions<DbContext>()).Model);
        var configuration = new CustomerConfiguration();
        // Act
        configuration.Configure(builder);

        // Assert
        Assert.True(builder.HasKey(x => x.Id).Metadata.IsPrimaryKey());
    }

    [Fact]
    public void Configure_Should_Set_Customer_Name_Max_Length()
    {
        // Arrange
        var builder = new EntityTypeBuilder<Customer>(new ModelBuilder(new DbContextOptions<DbContext>()).Model);
        var configuration = new CustomerConfiguration();

        // Act
        configuration.Configure(builder);

        // Assert
        Assert.Equal(100, builder.Property(x => x.Name).Metadata.GetMaxLength());
    }

    [Fact]
    public void Configure_Should_Set_Customer_Email_Max_Length()
    {
        // Arrange
        var builder = new EntityTypeBuilder<Customer>(new ModelBuilder(new DbContextOptions<DbContext>()).Model);
        var configuration = new CustomerConfiguration();

        // Act
        configuration.Configure(builder);

        // Assert
        Assert.Equal(100, builder.Property(x => x.Email).Metadata.GetMaxLength());
    }

    [Fact]
    public void Configure_Should_Set_Customer_Name_As_Required()
    {
        // Arrange
        var builder = new EntityTypeBuilder<Customer>(new ModelBuilder(new DbContextOptions<DbContext>()).Model);
        var configuration = new CustomerConfiguration();

        // Act
        configuration.Configure(builder);

        // Assert
        Assert.True(builder.Property(x => x.Name).Metadata.IsRequired);
    }

    [Fact]
    public void Configure_Should_Set_Customer_Email_As_Required()
    {
        // Arrange
        var builder = new EntityTypeBuilder<Customer>(new ModelBuilder(new DbContextOptions<DbContext>()).Model);
        var configuration = new CustomerConfiguration();

        // Act
        configuration.Configure(builder);

        // Assert
        Assert.True(builder.Property(x => x.Email).Metadata.IsRequired);
    }
}