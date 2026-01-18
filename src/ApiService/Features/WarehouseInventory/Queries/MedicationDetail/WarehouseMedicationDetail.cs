using System.Diagnostics;
using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.WarehouseInventory.Queries.MedicationDetail;

public sealed class WarehouseMedicationDetail(
    MeadDbContext db,
    ILogger<WarehouseMedicationDetail> logger)
    : Endpoint<WarehouseMedicationDetailRequest, WarehouseMedicationDetailResponse>
{
    public override void Configure()
    {
        Get("/{StockId:guid}");
        Group<WarehouseGroup>();
    }

    public override async Task HandleAsync(WarehouseMedicationDetailRequest req, CancellationToken ct)
    {
        using var activity = WarehouseTelemetry.ActivitySource.StartActivity("GetWarehouseMedicationDetail");
        var userId = User.GetUserId();

        if (activity?.IsAllDataRequested == true)
        {
            activity.SetTag("user.id", userId);
            activity.SetTag("stock.id", req.StockId);
            activity.SetTag("operation.name", "get_medication_detail");
        }

        logger.LogDebug(
            "Fetching medication detail for StockId {StockId}, user {UserId}",
            req.StockId,
            userId);

        try
        {
            var stock = await db.Stocks
                .AsNoTracking()
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
                activity.SetTag("stock.is_low", stock.HowManyLeft <= stock.WarnWhenBelow);
            }

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

            logger.LogDebug(
                "Retrieved medication detail: {MedicationName} (StockId: {StockId})",
                stock.Medication.Name,
                req.StockId);

            activity?.SetTag("result.status", "success");
            await Send.OkAsync(response, cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error fetching medication detail for StockId {StockId}, user {UserId}",
                req.StockId,
                userId);

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}