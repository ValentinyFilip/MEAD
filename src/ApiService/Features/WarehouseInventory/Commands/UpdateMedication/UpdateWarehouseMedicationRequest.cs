using Infrastructure.Domain.Medications.Enums;

namespace ApiService.Features.WarehouseInventory.Commands.UpdateMedication;

public sealed record UpdateWarehouseMedicationRequest(
    Guid StockId,
    decimal HowManyLeft,
    StockUnit Unit,
    DateTime? ExpiresOn,
    string? BatchNumber,
    string? WhereItsStored,
    decimal WarnWhenBelow
);