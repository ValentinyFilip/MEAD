using Coravel.Invocable;
using Infrastructure.Domain.Notifications.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.Notifications.Invocables;

public class CheckLowStockInvocable(
    MeadDbContext db,
    ILogger<CheckLowStockInvocable> logger) : IInvocable
{
    public async Task Invoke()
    {
        logger.LogInformation("Starting daily low stock check...");

        var lowStocks = await db.Stocks
            .Include(s => s.Medication)
            .Where(s => s.HowManyLeft <= s.WarnWhenBelow)
            .ToListAsync();

        if (lowStocks.Count == 0)
        {
             logger.LogInformation("No low stock items found.");
             return;
        }

        var lowStockIds = lowStocks.Select(s => s.Id).ToList();

        var recentNotificationStockIds = await db.Notifications
            .Where(n =>
                n.RelatedEntityId.HasValue &&
                lowStockIds.Contains(n.RelatedEntityId.Value) &&
                n.Type == NotificationType.LowStock &&
                n.CreatedAt > DateTime.UtcNow.AddHours(-24))
            .Select(n => n.RelatedEntityId!.Value)
            .ToListAsync();

        var recentlyNotifiedSet = new HashSet<Guid>(recentNotificationStockIds);

        int createdCount = 0;

        foreach (var stock in lowStocks)
        {
            if (!recentlyNotifiedSet.Contains(stock.Id))
            {
                var notification = new Infrastructure.Domain.Notifications.Notification
                {
                    UserId = stock.Medication.UserId,
                    Title = "Daily Low Stock Alert",
                    Message = $"Daily Reminder: Medication '{stock.Medication.Name}' is running low ({stock.HowManyLeft} {stock.Unit} remaining).",
                    Type = NotificationType.LowStock,
                    RelatedEntityId = stock.Id,
                    CreatedAt = DateTime.UtcNow
                };

                db.Notifications.Add(notification);
                createdCount++;
            }
        }

        if (createdCount > 0)
        {
            await db.SaveChangesAsync();
        }

        logger.LogInformation("Completed daily low stock check. Created {Count} new notifications.", createdCount);
    }
}
