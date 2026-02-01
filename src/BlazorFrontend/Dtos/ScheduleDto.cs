using Infrastructure.Domain.Medications.Enums;

namespace BlazorFrontend.Dtos;

public record ScheduleDto(
    Guid Id,
    Guid MedicationId,
    string MedicationName,
    decimal AmountPerTime,
    ScheduleUnit AmountUnit,
    FrequencyType HowOften,
    List<string> TimesPerDay,
    DateTime StartsOn,
    DateTime? EndsOn,
    bool IsActive,
    bool IsExpired
);