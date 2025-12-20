using FastEndpoints;
using FluentValidation;

namespace ApiService.Features.Auth.Commands.RegisterCommand;

public class RegisterValidator : Validator<RegisterRequest>
{
    public RegisterValidator()
    {
        // login nesmí být prázdný/whitespace
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithMessage("Login is required.")
            .Must(l => !string.IsNullOrWhiteSpace(l))
            .WithMessage("Login cannot be whitespace.");

        // heslo nesmí být prázdné/whitespace
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .Must(p => !string.IsNullOrWhiteSpace(p))
            .WithMessage("Password cannot be whitespace.");

        // confirm nesmí být prázdný/whitespace
        RuleFor(x => x.PasswordConfirm)
            .NotEmpty()
            .WithMessage("Password confirmation is required.")
            .Must(p => !string.IsNullOrWhiteSpace(p))
            .WithMessage("Password confirmation cannot be whitespace.");

        // hesla musí být stejná
        RuleFor(x => x)
            .Must(x => x.Password == x.PasswordConfirm)
            .WithMessage("Password and confirmation must match.");
    }
}