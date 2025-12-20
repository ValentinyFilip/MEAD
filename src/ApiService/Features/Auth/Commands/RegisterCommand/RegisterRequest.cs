namespace ApiService.Features.Auth.Commands.RegisterCommand;

public record RegisterRequest(string Login, string Password, string PasswordConfirm);