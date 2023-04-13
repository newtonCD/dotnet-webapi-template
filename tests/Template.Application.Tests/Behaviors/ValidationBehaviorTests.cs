using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Behaviors;
using CustomValidationException = Template.Application.Common.Exceptions.ValidationException;

namespace Template.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    public class TestRequest : IRequest<Unit> { }

    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator(bool shouldFail)
        {
            if (shouldFail)
            {
                RuleFor(x => x).Custom((_, context) =>
                {
                    context.AddFailure("Test failure");
                });
            }
        }
    }

    [Fact]
    public async Task ValidationBehavior_ValidationPasses_NoExceptionThrown()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>> { new TestRequestValidator(false) };
        var behavior = new ValidationBehavior<TestRequest, Unit>(validators);
        var request = new TestRequest();
        var requestHandlerDelegate = new RequestHandlerDelegate<Unit>(() => Task.FromResult(new Unit()));
        var cancellationToken = new CancellationToken();

        // Act
        var exception = await Record.ExceptionAsync(async () =>
            await behavior.Handle(request, requestHandlerDelegate, cancellationToken));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidationBehavior_ValidationFails_ValidationExceptionThrown()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>> { new TestRequestValidator(true) };
        var behavior = new ValidationBehavior<TestRequest, Unit>(validators);
        var request = new TestRequest();
        var requestHandlerDelegate = new RequestHandlerDelegate<Unit>(() => Task.FromResult(new Unit()));
        var cancellationToken = new CancellationToken();

        // Act
        var exception = await Record.ExceptionAsync(async () =>
            await behavior.Handle(request, requestHandlerDelegate, cancellationToken));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<CustomValidationException>(exception);
    }
}
