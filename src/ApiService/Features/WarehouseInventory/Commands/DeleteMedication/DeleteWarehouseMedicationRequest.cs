namespace ApiService.Features.WarehouseInventory.Commands.DeleteMedication;

public sealed record DeleteWarehouseMedicationRequest(
    Guid StockId
);