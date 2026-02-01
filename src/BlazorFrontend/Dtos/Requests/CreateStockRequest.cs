using Infrastructure.Domain.Medications.Enums;

namespace BlazorFrontend.Dtos.Requests;

public record CreateStockRequest(
    Guid MedicationId,
    decimal HowManyLeft,
    StockUnit Unit,
    DateTime? ExpiresOn,
    string? BatchNumber,
    string? WhereItsStored,
    decimal WarnWhenBelow
);