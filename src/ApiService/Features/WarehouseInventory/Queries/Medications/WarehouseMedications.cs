using System.Diagnostics;
using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.WarehouseInventory.Queries.Medications;

public sealed class WarehouseMedications(
    MeadDbContext db,
    ILogger<WarehouseMedications> logger)
    : EndpointWithoutRequest<List<WarehouseStockItem>>
{
    public override void Configure()
    {
        Get("");
        Group<WarehouseGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var activity = WarehouseTelemetry.ActivitySource.StartActivity("GetWarehouseMedications");
        var userId = User.GetUserId();

        if (activity?.IsAllDataRequested == true)
        {
            activity.SetTag("user.id", userId);
            activity.SetTag("operation.name", "list_medications");
        }

        logger.LogInformation("Fetching warehouse medications for user {UserId}", userId);

        try
        {
            var stocks = await db.Stocks
                .AsNoTracking()
                .Include(s => s.Medication)
                .Where(s => s.Medication.UserId == userId)
                .OrderBy(s => s.CreatedAt)
                .Select(s => new WarehouseStockItem(
                    s.Id,
                    s.MedicationId,
                    s.Medication.Name,
                    s.Medication.AlsoKnownAs,
                    s.HowManyLeft,
                    s.Unit,
                    s.ExpiresOn,
                    s.WhereItsStored,
                    s.HowManyLeft <= s.WarnWhenBelow,
                    s.CreatedAt
                ))
                .ToListAsync(ct);

            if (activity?.IsAllDataRequested == true)
            {
                activity.SetTag("result.count", stocks.Count);
                activity.SetTag("result.low_stock_count", stocks.Count(s => s.IsLowStock));
            }

            logger.LogInformation(
                "Retrieved {StockCount} medications for user {UserId}, {LowStockCount} items have low stock",
                stocks.Count,
                userId,
                stocks.Count(s => s.IsLowStock));

            // Record metrics
            var lowStockCount = stocks.Count(s => s.IsLowStock);
            if (lowStockCount > 0)
            {
                WarehouseTelemetry.StockLowCounter.Add(
                    lowStockCount,
                    new KeyValuePair<string, object?>("user.id", userId));
            }

            await Send.OkAsync(stocks, cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching warehouse medications for user {UserId}", userId);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}