using Infrastructure.Domain.Medications.Enums;

namespace BlazorFrontend.Dtos;

public record StockDto(
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
    bool IsLowStock,
    DateTime CreatedAt
);