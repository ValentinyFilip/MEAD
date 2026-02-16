using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.Notifications.Commands.MarkAsRead;

public sealed class MarkAsRead(MeadDbContext db) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Put("/{id}/read");
        Group<NotificationsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var userId = User.GetUserId();

        var notification = await db.Notifications
            .Where(n => n.Id == id && n.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (notification is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        notification.IsRead = true;
        await db.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}
