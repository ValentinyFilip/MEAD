using Infrastructure.Domain.Medications.Enums;
using Route = Infrastructure.Domain.Medications.Enums.Route;

namespace ApiService.Features.Medication.Commands.UpdateMedication;

public record UpdateMedicationRequest(
    Guid Id,
    string Name,
    string? AlsoKnownAs,
    DosageForm Form,
    decimal Strength,
    StrengthUnit StrengthUnit,
    Route HowToTake,
    string? ComesFrom,
    bool IsActive
);