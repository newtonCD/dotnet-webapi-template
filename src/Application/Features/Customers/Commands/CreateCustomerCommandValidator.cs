using Domain.Constants;
using FluentValidation;

namespace Application.Features.Customers.Commands;

/// <summary>
/// CreateCustomerCommandValidator é responsável por validar o comando CreateCustomerCommand.
/// </summary>
public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    /// <summary>
    /// Inicializa uma nova instância da classe CreateCustomerCommandValidator.
    /// </summary>
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationMessages.RequiredField);
        RuleFor(x => x.Email).NotEmpty().WithMessage(ValidationMessages.RequiredField);
    }
}