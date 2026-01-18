using System.Diagnostics;
using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.WarehouseInventory.Commands.DeleteMedication;

public sealed class DeleteWarehouseMedication(
    MeadDbContext db,
    ILogger<DeleteWarehouseMedication> logger)
    : Endpoint<DeleteWarehouseMedicationRequest>
{
    public override void Configure()
    {
        Delete("/{StockId:guid}");
        Group<WarehouseGroup>();
    }

    public override async Task HandleAsync(DeleteWarehouseMedicationRequest req, CancellationToken ct)
    {
        using var activity = WarehouseTelemetry.ActivitySource.StartActivity("DeleteWarehouseMedication");
        var userId = User.GetUserId();

        if (activity?.IsAllDataRequested == true)
        {
            activity.SetTag("user.id", userId);
            activity.SetTag("stock.id", req.StockId);
            activity.SetTag("operation.name", "delete_medication");
        }

        logger.LogInformation(
            "Deleting warehouse medication {StockId} for user {UserId}",
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

            if (activity?.IsAllDataRequested == true)
            {
                activity.SetTag("medication.id", stock.MedicationId);
                activity.SetTag("medication.name", stock.Medication.Name);
            }

            db.Stocks.Remove(stock);
            await db.SaveChangesAsync(ct);

            // Record metrics
            WarehouseTelemetry.MedicationDeletedCounter.Add(
                1,
                new KeyValuePair<string, object?>("user.id", userId),
                new KeyValuePair<string, object?>("medication.name", stock.Medication.Name));

            logger.LogInformation(
                "Successfully deleted medication {MedicationName} (StockId: {StockId}) for user {UserId}",
                stock.Medication.Name,
                req.StockId,
                userId);

            activity?.SetTag("result.status", "deleted");
            await Send.NoContentAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error deleting warehouse medication {StockId} for user {UserId}",
                req.StockId,
                userId);

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}