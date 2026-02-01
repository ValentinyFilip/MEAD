using Infrastructure.Domain.Medications.Enums;

namespace ApiService.Features.MedicationSchedule.Queries.MedicationWithSchedule;

public sealed record MedicationWithScheduleItem(
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