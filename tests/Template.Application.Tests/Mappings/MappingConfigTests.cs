using Application.Common.Mappings;
using Application.Common.Models.DTOs;
using Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.Tests.Mappings;

public class MappingConfigTests
{
    private readonly TypeAdapterConfig _typeAdapterConfig;

    public MappingConfigTests()
    {
        _typeAdapterConfig = new TypeAdapterConfig();
        new MappingConfig().Register(_typeAdapterConfig);
    }

    [Fact]
    public void CustomerToCustomerDtoMappingConfiguration_ShouldBeValid()
    {
        // Arrange
        var customer = new Customer("Test Name", "test@example.com");

        // Act
        var customerDto = customer.Adapt<CustomerDto>(_typeAdapterConfig);

        // Assert
        Assert.Equal(customer.Id, customerDto.Id);
        Assert.Equal(customer.Name, customerDto.Name);
        Assert.Equal(customer.Email, customerDto.Email);
    }

    [Fact]
    public void CustomerDtoToCustomerMappingConfiguration_ShouldBeValid()
    {
        // Arrange
        var customerDto = new CustomerDto { Id = 1, Name = "Test Name", Email = "test@example.com" };

        // Act
        var customer = customerDto.Adapt<Customer>(_typeAdapterConfig);

        // Assert
        Assert.Equal(customerDto.Id, customer.Id);
        Assert.Equal(customerDto.Name, customer.Name);
        Assert.Equal(customerDto.Email, customer.Email);
    }

    [Fact]
    public void MappingConfig_Register_ThrowsArgumentNullException_WhenConfigIsNull()
    {
        // Arrange
        var mappingConfig = new MappingConfig();
        TypeAdapterConfig config = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mappingConfig.Register(config));
    }
}
