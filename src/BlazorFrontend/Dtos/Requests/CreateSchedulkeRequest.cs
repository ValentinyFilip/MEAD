using Infrastructure.Domain.Medications.Enums;

namespace BlazorFrontend.Dtos.Requests;

public record CreateScheduleRequest(
    Guid MedicationId,
    decimal AmountPerTime,
    ScheduleUnit AmountUnit,
    FrequencyType HowOften,
    List<string> Times,
    DateTime StartsOn,
    DateTime? EndsOn
);