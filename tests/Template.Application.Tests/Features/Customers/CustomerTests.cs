using Template.Domain.Entities;

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