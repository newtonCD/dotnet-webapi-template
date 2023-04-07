using Domain.Constants;
using FluentValidation;

namespace Application.Features.Customers.Commands;

/// <summary>
/// UpdateCustomerCommandValidator é responsável por validar o comando UpdateCustomerCommand.
/// </summary>
public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    /// <summary>
    /// Inicializa uma nova instância da classe UpdateCustomerCommandValidator.
    /// </summary>
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationMessages.RequiredField);
        RuleFor(x => x.Email).NotEmpty().WithMessage(ValidationMessages.RequiredField);
    }
}