using ApiService.Common.Extensions;
using ApiService.Features.WarehouseInventory.Queries.MedicationDetail;
using FastEndpoints;
using Infrastructure.Domain.Medications;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.WarehouseInventory.Commands.AddMedication;

public sealed class AddWarehouseMedication(MeadDbContext db)
    : Endpoint<AddWarehouseMedicationRequest, WarehouseMedicationDetailResponse>
{
    public override void Configure()
    {
        Post("");
        Group<WarehouseGroup>();
    }

    public override async Task HandleAsync(AddWarehouseMedicationRequest req, CancellationToken ct)
    {
        var userId = User.GetUserId();

        var med = await db.Medications
            .AsNoTracking()
            .Where(m => m.Id == req.MedicationId && m.UserId == userId)
            .SingleOrDefaultAsync(ct);

        if (med is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var stock = new Stock
        {
            MedicationId = req.MedicationId,
            HowManyLeft = req.HowManyLeft,
            Unit = req.Unit,
            ExpiresOn = req.ExpiresOn,
            BatchNumber = req.BatchNumber,
            WhereItsStored = req.WhereItsStored,
            WarnWhenBelow = req.WarnWhenBelow,
            CreatedAt = DateTime.UtcNow
        };

        db.Stocks.Add(stock);

        if (req.HowManyLeft <= req.WarnWhenBelow)
        {
            var notification = new Infrastructure.Domain.Notifications.Notification
            {
                UserId = userId,
                Title = "Low Stock Alert",
                Message = $"Medication '{med.Name}' is running low ({req.HowManyLeft} {req.Unit} remaining).",
                Type = Infrastructure.Domain.Notifications.Enums.NotificationType.LowStock,
                RelatedEntityId = stock.Id,
                CreatedAt = DateTime.UtcNow
            };
            db.Notifications.Add(notification);
        }

        await db.SaveChangesAsync(ct);

        var response = new WarehouseMedicationDetailResponse(
            stock.Id,
            med.Id,
            med.Name,
            med.AlsoKnownAs,
            stock.HowManyLeft,
            stock.Unit,
            stock.ExpiresOn,
            stock.BatchNumber,
            stock.WhereItsStored,
            stock.WarnWhenBelow,
            stock.CreatedAt,
            stock.UpdatedAt
        );

        await Send.OkAsync(response, cancellation: ct);
    }
}