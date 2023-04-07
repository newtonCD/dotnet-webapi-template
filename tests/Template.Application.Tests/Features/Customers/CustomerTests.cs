using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.Tests.Features.Customers;

public class CustomerTests
{
    [Fact]
    public void InternalConstructor_ShouldInitializeProperties()
    {
        // Arrange & Act
        var customer = new Customer();

        // Assert
        Assert.NotNull(customer);
        Assert.Equal(0, customer.Id);
        Assert.Null(customer.Name);
        Assert.Null(customer.Email);
    }
}