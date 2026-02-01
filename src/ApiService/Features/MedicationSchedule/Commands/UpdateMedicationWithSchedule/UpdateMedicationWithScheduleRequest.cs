using Infrastructure.Domain.Medications.Enums;

namespace ApiService.Features.MedicationSchedule.Commands.UpdateMedicationWithSchedule;

public sealed record UpdateMedicationWithScheduleRequest(
    Guid Id,
    decimal AmountPerTime,
    ScheduleUnit AmountUnit,
    FrequencyType HowOften,
    List<string> Times,
    DateTime StartsOn,
    DateTime? EndsOn,
    bool IsActive
);