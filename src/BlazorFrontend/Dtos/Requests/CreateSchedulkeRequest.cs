namespace BlazorFrontend.Dtos.Requests;

public record CreateScheduleRequest(
    Guid MedicationId,
    decimal AmountPerTime,
    string AmountUnit,
    string HowOften,
    List<string> Times,
    DateTime StartsOn,
    DateTime? EndsOn
);