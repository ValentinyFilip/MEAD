namespace ApiService.Features.MedicationSchedule.Queries.MedicationWithSchedule;

public sealed record MedicationWithSchedulesResponse(
    List<MedicationWithScheduleItem> Items
);