using Domain.Constants;
using FluentValidation;
using System.Globalization;

namespace Application.Features.Base;

/// <summary>
/// Classe genérica de validação de consulta de entidades.
/// </summary>
/// <typeparam name="TQuery">Tipo da consulta a ser validada.</typeparam>
public class GetPagingQueryValidator<TQuery> : AbstractValidator<TQuery>
    where TQuery : IGetPagingQuery
{
    private const int MaxPageSize = 50;

    /// <summary>
    /// Construtor que define as regras de validação para a consulta.
    /// </summary>
    public GetPagingQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage(ValidationMessages.InvalidPageNumber);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, MaxPageSize)
            .WithMessage(string.Format(CultureInfo.InvariantCulture, ValidationMessages.InvalidPageSize, MaxPageSize));
    }
}