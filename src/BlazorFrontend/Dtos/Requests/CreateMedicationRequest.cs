namespace BlazorFrontend.Dtos.Requests;

public record CreateMedicationRequest(
    string Name,
    string? AlsoKnownAs,
    string Form,
    decimal Strength,
    string StrengthUnit,
    string HowToTake,
    string? ComesFrom
);