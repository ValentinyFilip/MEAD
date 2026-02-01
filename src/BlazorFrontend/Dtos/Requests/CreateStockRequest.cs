namespace BlazorFrontend.Dtos.Requests;

public record CreateStockRequest(
    Guid MedicationId,
    decimal HowManyLeft,
    string Unit,
    DateTime? ExpiresOn,
    string? BatchNumber,
    string? WhereItsStored,
    decimal WarnWhenBelow
);