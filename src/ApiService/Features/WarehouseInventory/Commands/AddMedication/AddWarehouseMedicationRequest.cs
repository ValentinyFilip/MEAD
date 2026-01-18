using Infrastructure.Domain.Medications.Enums;

namespace ApiService.Features.WarehouseInventory.Commands.AddMedication;

public sealed record AddWarehouseMedicationRequest(
    Guid MedicationId,
    decimal HowManyLeft,
    StockUnit Unit,
    DateTime? ExpiresOn,
    string? BatchNumber,
    string? WhereItsStored,
    decimal WarnWhenBelow
);