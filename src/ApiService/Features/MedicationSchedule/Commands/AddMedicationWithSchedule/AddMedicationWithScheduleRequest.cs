using Infrastructure.Domain.Medications.Enums;

namespace ApiService.Features.MedicationSchedule.Commands.AddMedicationWithSchedule;

public sealed record AddMedicationWithScheduleRequest(
    Guid MedicationId,
    decimal AmountPerTime,
    ScheduleUnit AmountUnit,
    FrequencyType HowOften,
    List<string> Times,
    DateTime StartsOn,
    DateTime? EndsOn
);