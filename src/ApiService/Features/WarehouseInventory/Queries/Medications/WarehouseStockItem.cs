using Infrastructure.Domain.Medications.Enums;

namespace ApiService.Features.WarehouseInventory.Queries.Medications;

public sealed record WarehouseStockItem(
    Guid StockId,
    Guid MedicationId,
    string MedicationName,
    string? AlsoKnownAs,
    decimal HowManyLeft,
    StockUnit Unit,
    DateTime? ExpiresOn,
    string? WhereItsStored,
    bool IsLowStock, // computed: HowManyLeft <= WarnWhenBelow
    DateTime CreatedAt
);