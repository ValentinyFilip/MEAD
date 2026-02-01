namespace BlazorFrontend.Dtos;

public record ScheduleDto(
    Guid Id,
    Guid MedicationId,
    string MedicationName,
    decimal AmountPerTime,
    string AmountUnit,
    string HowOften,
    int TimesPerDay,
    DateTime StartsOn,
    DateTime? EndsOn,
    bool IsActive,
    bool IsExpired
);