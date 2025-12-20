using System.Diagnostics;
using System.Security.Claims;
using ApiService.Common;
using ApiService.Common.Extensions;
using ApiService.Configuration;
using ApiService.Features.Auth.Services;
using FastEndpoints;
using FastEndpoints.Security;
using Infrastructure.Domain.Auth;
using Infrastructure.Domain.User;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiService.Features.Auth.Commands.RegisterCommand;

public class RegisterCommand(MeadDbContext dbContext, IOptions<JwtConfiguration> configuration, ILogger<RegisterCommand> logger)
    : Endpoint<RegisterRequest, TokenResponse>
{
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        using var activity = AuthTelemetry.ActivitySource.StartActivity("Auth.Register", ActivityKind.Server);
        activity?.SetTag("auth.register", req.Login);
        
        // existující uživatel
        if (await dbContext.Users.AnyAsync(u => u.Email == req.Login, ct))
        {
            activity?.SetTag("auth.register.exists", true);
            logger.LogWarning("Registration failed for {Email}: user already exists", req.Login);

            await Send.ConflictAsync(new { message = "User with this email already exists." }, ct);
            return;
        }

        var user = new User
        {
            Email = req.Login,
            PasswordHash = HashHelper.Sha256(req.Password),
            LastLoginAt = DateTime.UtcNow,
            Roles = [UserRole.User]
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        AuthTelemetry.RegisterCounter.Add(1);

        activity?.SetTag("auth.register.success", true);
        activity?.SetTag("auth.user_id", user.Id);

        logger.LogInformation("User {UserId} registered successfully", user.Id);

        var tokenResponse = await CreateTokenWith<AuthTokenService>(
            user.Id.ToString(),
            privileges =>
            {
                privileges.Roles.AddRange(user.Roles.Select(r => r.ToString()));
                privileges.Claims.Add(new Claim("sub", user.Id.ToString()));
                privileges.Claims.Add(new Claim("email", user.Email));
                privileges.Claims.Add(new Claim("issuer", configuration.Value.Issuer));
                privileges.Claims.Add(new Claim("audience", configuration.Value.Audience));
            });

        await Send.OkAsync(tokenResponse, ct);
    }
}