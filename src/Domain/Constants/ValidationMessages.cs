namespace Domain.Constants;

public static class ValidationMessages
{
    public const string RequiredField = "O campo '{PropertyName}' é obrigatório.";
    public const string InvalidEmailFormat = "O campo '{PropertyName}' deve ser um endereço de e-mail válido.";
    public const string StringLength = "O campo '{PropertyName}' deve ter entre {MinLength} e {MaxLength} caracteres.";
    public const string InvalidPhoneNumberFormat = "O campo '{PropertyName}' deve ser um número de telefone válido.";
    public const string InvalidPageNumber = "O número da página deve ser maior ou igual a 1.";
    public const string InvalidPageSize = "O tamanho da página deve estar entre 1 e {0}.";
}