using Infrastructure.Domain.Notifications.Enums;

namespace Infrastructure.Domain.Notifications;

public class Notification
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid UserId { get; set; }
    public User.User User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public NotificationType Type { get; set; }
    public Guid? RelatedEntityId { get; set; } // Could be StockId
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
