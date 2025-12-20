using System.Diagnostics;
using ApiService.Common;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.Auth.Commands.LogoutCommand;

public class LogoutCommand(MeadDbContext dbContext, ILogger<LogoutCommand> logger) : Endpoint<EmptyRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/auth/logout");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        using var activity = AuthTelemetry.ActivitySource.StartActivity("Auth.Logout", ActivityKind.Server);

        AuthTelemetry.LogoutCounter.Add(1);

        if (HttpContext.Request.Cookies.TryGetValue("refresh_token", out var rawRefresh)
            && !string.IsNullOrWhiteSpace(rawRefresh))
        {
            var hash = HashHelper.Sha256(rawRefresh);

            var deleted = await dbContext.RefreshTokens
                .Where(t => t.TokenHash == hash)
                .ExecuteDeleteAsync(ct);

            activity?.SetTag("auth.logout.tokens_deleted", deleted);
            logger.LogInformation("Logout: deleted {Count} refresh tokens", deleted);
        }
        else
        {
            activity?.SetTag("auth.logout.tokens_deleted", 0);
            logger.LogInformation("Logout: no refresh token cookie found");
        }

        HttpContext.Response.Cookies.Delete("refresh_token");

        await Send.OkAsync(ct);
    }
}