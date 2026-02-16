using Infrastructure.Domain.Notifications.Enums;

namespace ApiService.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsResponse(
    Guid Id,
    string Title,
    string Message,
    bool IsRead,
    NotificationType Type,
    Guid? RelatedEntityId,
    DateTime CreatedAt
);
