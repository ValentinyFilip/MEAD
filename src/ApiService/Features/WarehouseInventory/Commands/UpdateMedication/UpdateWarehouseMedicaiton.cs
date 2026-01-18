using System.Diagnostics;
using ApiService.Common.Extensions;
using ApiService.Features.WarehouseInventory.Queries.MedicationDetail;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.WarehouseInventory.Commands.UpdateMedication;

public sealed class UpdateWarehouseMedication(
    MeadDbContext db,
    ILogger<UpdateWarehouseMedication> logger)
    : Endpoint<UpdateWarehouseMedicationRequest, WarehouseMedicationDetailResponse>
{
    public override void Configure()
    {
        Put("/{StockId:guid}");
        Group<WarehouseGroup>();
    }

    public override async Task HandleAsync(UpdateWarehouseMedicationRequest req, CancellationToken ct)
    {
        using var activity = WarehouseTelemetry.ActivitySource.StartActivity("UpdateWarehouseMedication");
        var userId = User.GetUserId();

        if (activity?.IsAllDataRequested == true)
        {
            activity.SetTag("user.id", userId);
            activity.SetTag("stock.id", req.StockId);
            activity.SetTag("operation.name", "update_medication");
        }

        logger.LogInformation(
            "Updating warehouse medication {StockId} for user {UserId}",
            req.StockId,
            userId);

        try
        {
            var stock = await db.Stocks
                .Include(s => s.Medication)
                .Where(s => s.Id == req.StockId && s.Medication.UserId == userId)
                .SingleOrDefaultAsync(ct);

            if (stock is null)
            {
                logger.LogWarning(
                    "Stock item {StockId} not found for user {UserId}",
                    req.StockId,
                    userId);

                activity?.SetTag("result.status", "not_found");
                await Send.NotFoundAsync(ct);
                return;
            }

            var previousQuantity = stock.HowManyLeft;
            var quantityChanged = previousQuantity != req.HowManyLeft;

            stock.HowManyLeft = req.HowManyLeft;
            stock.Unit = req.Unit;
            stock.ExpiresOn = req.ExpiresOn;
            stock.BatchNumber = req.BatchNumber;
            stock.WhereItsStored = req.WhereItsStored;
            stock.WarnWhenBelow = req.WarnWhenBelow;
            stock.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);

            if (activity?.IsAllDataRequested == true)
            {
                activity.SetTag("medication.id", stock.MedicationId);
                activity.SetTag("medication.name", stock.Medication.Name);
                activity.SetTag("stock.quantity_changed", quantityChanged);
                activity.SetTag("stock.quantity_delta", req.HowManyLeft - previousQuantity);
                activity.SetTag("stock.is_low", req.HowManyLeft <= req.WarnWhenBelow);
            }

            // Record metrics
            WarehouseTelemetry.MedicationUpdatedCounter.Add(
                1,
                new KeyValuePair<string, object?>("user.id", userId),
                new KeyValuePair<string, object?>("medication.name", stock.Medication.Name));

            if (quantityChanged)
            {
                WarehouseTelemetry.StockAdjustedCounter.Add(
                    1,
                    new KeyValuePair<string, object?>("user.id", userId),
                    new KeyValuePair<string, object?>("adjustment.type",
                        req.HowManyLeft > previousQuantity ? "increase" : "decrease"));
            }

            if (req.HowManyLeft <= req.WarnWhenBelow)
            {
                logger.LogWarning(
                    "Low stock alert: {MedicationName} has {Quantity} {Unit} remaining (threshold: {Threshold})",
                    stock.Medication.Name,
                    req.HowManyLeft,
                    req.Unit,
                    req.WarnWhenBelow);

                WarehouseTelemetry.StockLowCounter.Add(
                    1,
                    new KeyValuePair<string, object?>("user.id", userId),
                    new KeyValuePair<string, object?>("medication.name", stock.Medication.Name));
            }

            logger.LogInformation(
                "Successfully updated medication {MedicationName} (StockId: {StockId}), quantity: {OldQuantity} → {NewQuantity} {Unit}",
                stock.Medication.Name,
                req.StockId,
                previousQuantity,
                req.HowManyLeft,
                req.Unit);

            var response = new WarehouseMedicationDetailResponse(
                stock.Id,
                stock.MedicationId,
                stock.Medication.Name,
                stock.Medication.AlsoKnownAs,
                stock.HowManyLeft,
                stock.Unit,
                stock.ExpiresOn,
                stock.BatchNumber,
                stock.WhereItsStored,
                stock.WarnWhenBelow,
                stock.CreatedAt,
                stock.UpdatedAt
            );

            activity?.SetTag("result.status", "updated");
            await Send.OkAsync(response, cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error updating warehouse medication {StockId} for user {UserId}",
                req.StockId,
                userId);

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}