using Xunit;
using FluentValidation.TestHelper;
using Application.Features.Customers.Commands;
using Application.Features.Customers.Queries;

namespace Template.Application.Tests.Features.Customers.Queries;

public class GetCustomersQueryValidatorTests
{
    private readonly GetCustomersQueryValidator _validator;

    public GetCustomersQueryValidatorTests()
    {
        _validator = new GetCustomersQueryValidator();
    }

    [Fact]
    public void PageNumber_ShouldNotHaveValidationErrorWhen_GreaterThanZero()
    {
        // Act
        var result = _validator.TestValidate(new GetCustomersQuery(1, 10));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void PageSize_ShouldNotHaveValidationErrorWhen_GreaterThanZero()
    {
        // Act
        var result = _validator.TestValidate(new GetCustomersQuery(1, 10));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void PageNumber_ShouldHaveValidationErrorWhen_LessThanOrEqualToZero()
    {
        // Act
        var result = _validator.TestValidate(new GetCustomersQuery(0, 10));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void PageSize_ShouldHaveValidationErrorWhen_LessThanOrEqualToZero()
    {
        // Act
        var result = _validator.TestValidate(new GetCustomersQuery(1, 0));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}