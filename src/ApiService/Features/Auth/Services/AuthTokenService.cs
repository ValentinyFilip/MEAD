using System.Security.Claims;
using ApiService.Common;
using ApiService.Configuration;
using FastEndpoints;
using FastEndpoints.Security;
using Infrastructure.Domain.Auth;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiService.Features.Auth.Services;

public class AuthTokenService : RefreshTokenService<TokenRequest, TokenResponse>
{
    private readonly MeadDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<JwtConfiguration> _config;

    public AuthTokenService(IOptions<JwtConfiguration> config, MeadDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _config = config;

        Setup(o =>
        {
            o.TokenSigningKey = config.Value.SigningKey;
            o.AccessTokenValidity = TimeSpan.FromMinutes(config.Value.AccessExpirationInMinutes);
            o.RefreshTokenValidity = TimeSpan.FromDays(config.Value.RefreshExpirationInDays);
            o.Endpoint("/auth/refresh", ep =>
            {
                ep.Summary(s => s.Summary = "Refresh JWT token pair.");
                ep.AllowAnonymous();
            });
        });
    }

    public override async Task RefreshRequestValidationAsync(TokenRequest req)
    {
        var http = _httpContextAccessor.HttpContext!;
        if (!http.Request.Cookies.TryGetValue("refresh_token", out var rawRefresh)
            || string.IsNullOrWhiteSpace(rawRefresh))
        {
            AddError(r => r.RefreshToken, "Missing refresh token cookie.");
            return;
        }

        var hash = HashHelper.Sha256(rawRefresh);

        var token = await _db.RefreshTokens
            .SingleOrDefaultAsync(t => t.TokenHash == hash);

        if (token is null || token.ExpiresAt <= DateTime.UtcNow)
        {
            AddError(r => r.RefreshToken, "Refresh token is invalid or expired.");
            return;
        }

        req.UserId = token.UserId.ToString();
    }

    public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
    {
        var userId = Guid.Parse(request.UserId);
        var user = await _db.Users.SingleAsync(u => u.Id == userId);

        privileges.Roles.AddRange(user.Roles.Select(r => r.ToString()));
        privileges.Claims.Add(new Claim("sub", user.Id.ToString()));
        privileges.Claims.Add(new Claim("email", user.Email));
        privileges.Claims.Add(new Claim("issuer", _config.Value.Issuer));
        privileges.Claims.Add(new Claim("audience", _config.Value.Audience));
    }

    public override async Task PersistTokenAsync(TokenResponse response)
    {
        var ctx = _httpContextAccessor.HttpContext!;
        var userId = Guid.Parse(response.UserId);

        var rawRefresh = response.RefreshToken;
        var hash = HashHelper.Sha256(rawRefresh);

        var token = await _db.RefreshTokens.SingleOrDefaultAsync(t => t.UserId == userId);
        if (token is null)
        {
            token = new RefreshToken
            {
                UserId = userId,
                TokenHash = hash,
                ExpiresAt = response.RefreshExpiry,
                CreatedByIp = ctx.Connection.RemoteIpAddress?.ToString()
            };
            _db.RefreshTokens.Add(token);
        }
        else
        {
            token.TokenHash = hash;
            token.ExpiresAt = response.RefreshExpiry;
            token.CreatedAt = DateTime.UtcNow;
            token.CreatedByIp = ctx.Connection.RemoteIpAddress?.ToString();
        }

        await _db.SaveChangesAsync();

        ctx.Response.Cookies.Append(
            "refresh_token",
            rawRefresh,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = response.RefreshExpiry
            });

        response.RefreshToken = string.Empty;
    }
}