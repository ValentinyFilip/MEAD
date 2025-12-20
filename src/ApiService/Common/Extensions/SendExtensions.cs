using FastEndpoints;

namespace ApiService.Common.Extensions;

static class SendExtensions
{
    public static Task ConflictAsync<T>(this IResponseSender sender, T body, CancellationToken ct = default)
        => sender.HttpContext.Response.SendAsync(body, StatusCodes.Status409Conflict, cancellation: ct);
}