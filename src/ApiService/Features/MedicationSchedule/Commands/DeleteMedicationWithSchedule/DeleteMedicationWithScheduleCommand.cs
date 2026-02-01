using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Domain.User;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.MedicationSchedule.Commands.DeleteMedicationWithSchedule;

public class DeleteMedicationWithScheduleCommand(MeadDbContext db)
    : Endpoint<DeleteMedicationWithScheduleRequest>
{
    public override void Configure()
    {
        Delete("/{Id:guid}");
        Group<MedicationSchedulesGroup>();
    }

    public override async Task HandleAsync(DeleteMedicationWithScheduleRequest req, CancellationToken ct)
    {
        using var activity = SchedulesTelemetry.ActivitySource.StartActivity("DeleteMedicationSchedule");

        var userId = User.GetUserId();
        activity?.SetTag("user.id", userId);
        activity?.SetTag("schedule.id", req.Id);

        var schedule = await db.MedicationSchedules
            .Include(s => s.Medication)
            .Where(s => s.Id == req.Id && s.Medication.UserId == userId)
            .SingleOrDefaultAsync(ct);

        if (schedule is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var wasActive = schedule.IsActive;

        db.MedicationSchedules.Remove(schedule);
        await db.SaveChangesAsync(ct);

        SchedulesTelemetry.ScheduleDeletedCounter.Add(1,
            new KeyValuePair<string, object?>("user.id", userId),
            new KeyValuePair<string, object?>("schedule.id", req.Id));

        if (wasActive)
        {
            SchedulesTelemetry.TotalActiveSchedulesCounter.Add(-1);
        }

        await Send.NoContentAsync(ct);
    }
}