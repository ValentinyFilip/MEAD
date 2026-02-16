using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.Notifications.Queries.GetNotifications;

public sealed class GetNotifications(MeadDbContext db)
    : EndpointWithoutRequest<List<GetNotificationsResponse>>
{
    public override void Configure()
    {
        Get("");
        Group<NotificationsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.GetUserId();

        var notifications = await db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new GetNotificationsResponse(
                n.Id,
                n.Title,
                n.Message,
                n.IsRead,
                n.Type,
                n.RelatedEntityId,
                n.CreatedAt
            ))
            .ToListAsync(ct);

        await Send.OkAsync(notifications, ct);
    }
}
