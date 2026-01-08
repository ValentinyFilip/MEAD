using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ApiService.Features.WarehouseInventory;

public static class WarehouseTelemetry
{
    public static readonly ActivitySource ActivitySource = new("Mead.Warehouse");
    public static readonly Meter Meter = new("Mead.Warehouse.Meter");

    // Medication operations
    public static readonly Counter<long> MedicationCreatedCounter =
        Meter.CreateCounter<long>("mead_warehouse_medications_created_total", "medications", "Number of medications created");

    public static readonly Counter<long> MedicationUpdatedCounter =
        Meter.CreateCounter<long>("mead_warehouse_medications_updated_total", "medications", "Number of medications updated");

    public static readonly Counter<long> MedicationDeletedCounter =
        Meter.CreateCounter<long>("mead_warehouse_medications_deleted_total", "medications", "Number of medications deleted");

    // Stock operations
    public static readonly Counter<long> StockCreatedCounter =
        Meter.CreateCounter<long>("mead_warehouse_stocks_created_total", "stock_items", "Number of stock items created");

    public static readonly Counter<long> StockAdjustedCounter =
        Meter.CreateCounter<long>("mead_warehouse_stock_adjusted_total", "stock_adjustments", "Number of stock quantity adjustments");

    public static readonly Counter<long> StockLowCounter =
        Meter.CreateCounter<long>("mead_warehouse_stock_low_total", "low_stock_alerts", "Number of low stock alerts triggered");

    public static readonly Counter<long> StockExpiredCounter =
        Meter.CreateCounter<long>("mead_warehouse_stock_expired_total", "expired_stock", "Number of expired stock items found");

    // Warehouse views/business metrics
    public static readonly UpDownCounter<long> TotalMedicationsCounter =
        Meter.CreateUpDownCounter<long>("mead_warehouse_total_medications", "medications", "Total number of medications");

    public static readonly UpDownCounter<long> TotalStockQuantityCounter =
        Meter.CreateUpDownCounter<long>("mead_warehouse_total_stock_quantity", "units", "Total stock quantity across all medications");

    public static readonly Histogram<double> AverageStockDaysCounter =
        Meter.CreateHistogram<double>("mead_warehouse_avg_stock_days", "days", "Average days of stock remaining");
}