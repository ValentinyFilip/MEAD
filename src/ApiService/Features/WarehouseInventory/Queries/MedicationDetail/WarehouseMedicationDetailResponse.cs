using Infrastructure.Domain.Medications.Enums;

namespace ApiService.Features.WarehouseInventory.Queries.MedicationDetail;

public sealed record WarehouseMedicationDetailResponse(
    Guid StockId,
    Guid MedicationId,
    string MedicationName,
    string? AlsoKnownAs,
    decimal HowManyLeft,
    StockUnit Unit,
    DateTime? ExpiresOn,
    string? BatchNumber,
    string? WhereItsStored,
    decimal WarnWhenBelow,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);