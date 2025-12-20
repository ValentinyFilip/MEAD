using FastEndpoints;
using FluentValidation;

namespace ApiService.Features.Auth.Commands.LoginCommand;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithMessage("Login is required.")
            .Must(l => !string.IsNullOrWhiteSpace(l))
            .WithMessage("Login cannot be whitespace.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .Must(p => !string.IsNullOrWhiteSpace(p))
            .WithMessage("Password cannot be whitespace.");
    }
}