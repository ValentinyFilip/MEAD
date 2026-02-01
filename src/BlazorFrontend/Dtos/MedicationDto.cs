using Infrastructure.Domain.Medications.Enums;
using Route = Infrastructure.Domain.Medications.Enums.Route;

namespace BlazorFrontend.Dtos;

public record MedicationDto(
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