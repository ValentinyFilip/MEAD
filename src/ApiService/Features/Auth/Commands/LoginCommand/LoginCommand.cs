using System.Diagnostics;
using System.Security.Claims;
using ApiService.Common;
using ApiService.Configuration;
using ApiService.Features.Auth.Services;
using FastEndpoints;
using FastEndpoints.Security;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiService.Features.Auth.Commands.LoginCommand;

public class LoginCommand(MeadDbContext dbContext, IOptions<JwtConfiguration> configuration, ILogger<LoginCommand> logger) : Endpoint<LoginRequest, TokenResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        using var activity = AuthTelemetry.ActivitySource.StartActivity("Auth.Login", ActivityKind.Server);
        activity?.SetTag("auth.login", req.Login);

        AuthTelemetry.LoginCounter.Add(1);

        var user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == req.Login, ct);

        if (user is null || !HashHelper.Sha256(req.Password).Equals(user.PasswordHash, StringComparison.InvariantCulture))
        {
            AuthTelemetry.LoginFailedCounter.Add(1);
            activity?.SetTag("auth.login.success", false);
            logger.LogWarning("Login failed for {Login}", req.Login);

            await Send.UnauthorizedAsync(ct);
            return;
        }

        activity?.SetTag("auth.login.success", true);
        activity?.SetTag("auth.user_id", user.Id);

        logger.LogInformation("User {UserId} logged in successfully", user.Id);

        user.LastLoginAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);

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