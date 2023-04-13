using FluentValidation.TestHelper;
using Template.Application.Features.Customers.Commands;

namespace Template.Application.Tests.Features.Customers.Commands;

public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator;

    public CreateCustomerCommandValidatorTests()
    {
        _validator = new CreateCustomerCommandValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_InvalidName_ShouldHaveValidationError(string invalidName)
    {
        // Act
        var result = _validator.TestValidate(new CreateCustomerCommand(invalidName, "john.doe@example.com"));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ValidName_ShouldNotHaveValidationError()
    {
        // Act
        var result = _validator.TestValidate(new CreateCustomerCommand("John Doe", "john.doe@example.com"));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_InvalidEmail_ShouldHaveValidationError(string invalidEmail)
    {
        // Act
        var result = _validator.TestValidate(new CreateCustomerCommand("John Doe", invalidEmail));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ValidEmail_ShouldNotHaveValidationError()
    {
        // Act
        var result = _validator.TestValidate(new CreateCustomerCommand("John Doe", "john.doe@example.com"));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}