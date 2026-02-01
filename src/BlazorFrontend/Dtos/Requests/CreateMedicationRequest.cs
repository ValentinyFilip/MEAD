using Infrastructure.Domain.Medications.Enums;
using Route = Infrastructure.Domain.Medications.Enums.Route;

namespace BlazorFrontend.Dtos.Requests;

public record CreateMedicationRequest(
    string Name,
    string? AlsoKnownAs,
    DosageForm Form,
    decimal Strength,
    StrengthUnit StrengthUnit,
    Route HowToTake,
    string? ComesFrom
);