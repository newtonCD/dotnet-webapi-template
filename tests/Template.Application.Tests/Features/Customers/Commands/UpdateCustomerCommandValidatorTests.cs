using Application.Features.Customers.Commands;
using FluentValidation.TestHelper;

namespace Template.Application.Tests.Features.Customers.Commands;

public class UpdateCustomerCommandValidatorTests
{
    private readonly UpdateCustomerCommandValidator _validator;

    public UpdateCustomerCommandValidatorTests()
    {
        _validator = new UpdateCustomerCommandValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_InvalidName_ShouldHaveValidationError(string invalidName)
    {
        // Act
        var result = _validator.TestValidate(new UpdateCustomerCommand(1, invalidName, "john.doe@example.com"));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ValidName_ShouldNotHaveValidationError()
    {
        // Act
        var result = _validator.TestValidate(new UpdateCustomerCommand(1, "John Doe", "john.doe@example.com"));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_InvalidEmail_ShouldHaveValidationError(string invalidEmail)
    {
        // Act
        var result = _validator.TestValidate(new UpdateCustomerCommand(1, "John Doe", invalidEmail));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ValidEmail_ShouldNotHaveValidationError()
    {
        // Act
        var result = _validator.TestValidate(new UpdateCustomerCommand(1, "John Doe", "john.doe@example.com"));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}