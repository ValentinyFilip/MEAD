using FastEndpoints;

namespace ApiService.Features.Notifications;

public sealed class NotificationsGroup : Group
{
    public NotificationsGroup()
    {
        Configure("/notifications", ep =>
        {
            ep.Description(d => d
                .ProducesProblemDetails(401)
                .ProducesProblemDetails(403)
                .ProducesProblemDetails(404)
                .ProducesProblemDetails(500)
                .WithTags("Notifications")
                .WithGroupName("Notifications"));
        });
    }
}
