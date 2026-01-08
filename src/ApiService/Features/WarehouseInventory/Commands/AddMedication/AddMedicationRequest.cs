using Infrastructure.Domain.Medications.Enums;
using Route = Microsoft.AspNetCore.Routing.Route;

namespace ApiService.Features.WarehouseInventory.Commands.AddMedication;

public record AddMedicationRequest(
    string Name,
    string? AlsoKnownAs,
    DosageForm Form,
    decimal Strength,
    StrengthUnit StrengthUnit,
    Route HowToTake,
    string? ComesFrom,
    decimal HowManyLeft,
    StockUnit Unit,
    DateTime? ExpiresOn,
    string? BatchNumber,
    string? WhereItsStored,
    decimal WarnWhenBelow
);