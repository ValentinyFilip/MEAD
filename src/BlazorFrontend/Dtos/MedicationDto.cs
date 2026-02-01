namespace BlazorFrontend.Dtos;

public record MedicationDto(
    Guid Id,
    string Name,
    string? AlsoKnownAs,
    string Form,
    decimal Strength,
    string StrengthUnit,
    string HowToTake,
    string? ComesFrom,
    bool IsActive
);