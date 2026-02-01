using Infrastructure.Domain.Medications.Enums;

namespace ApiService.Features.MedicationSchedule;

public sealed record MedicationWithScheduleResponse(
    Guid Id,
    Guid MedicationId,
    string MedicationName,
    decimal AmountPerTime,
    ScheduleUnit AmountUnit,
    FrequencyType HowOften,
    List<string> Times,
    DateTime StartsOn,
    DateTime? EndsOn,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);