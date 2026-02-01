using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Domain.User;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.MedicationSchedule.Commands.UpdateMedicationWithSchedule;

public class UpdateMedicationWithScheduleCommand(MeadDbContext db)
    : Endpoint<UpdateMedicationWithScheduleRequest, MedicationWithScheduleResponse>
{
    public override void Configure()
    {
        Put("/{Id:guid}");
        Group<MedicationSchedulesGroup>();
    }

    public override async Task HandleAsync(UpdateMedicationWithScheduleRequest req, CancellationToken ct)
    {
        using var activity = SchedulesTelemetry.ActivitySource.StartActivity("UpdateMedicationSchedule");

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

        schedule.AmountPerTime = req.AmountPerTime;
        schedule.AmountUnit = req.AmountUnit;
        schedule.HowOften = req.HowOften;
        schedule.Times = req.Times;
        schedule.StartsOn = req.StartsOn;
        schedule.EndsOn = req.EndsOn;
        schedule.IsActive = req.IsActive;
        schedule.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        SchedulesTelemetry.ScheduleUpdatedCounter.Add(1,
            new KeyValuePair<string, object?>("user.id", userId),
            new KeyValuePair<string, object?>("schedule.id", req.Id));

        // Track activation state changes
        if (!wasActive && req.IsActive)
        {
            SchedulesTelemetry.ScheduleReactivatedCounter.Add(1);
            SchedulesTelemetry.TotalActiveSchedulesCounter.Add(1);
        }
        else if (wasActive && !req.IsActive)
        {
            SchedulesTelemetry.ScheduleDeactivatedCounter.Add(1);
            SchedulesTelemetry.TotalActiveSchedulesCounter.Add(-1);
        }

        var response = new MedicationWithScheduleResponse(
            schedule.Id,
            schedule.MedicationId,
            schedule.Medication.Name,
            schedule.AmountPerTime,
            schedule.AmountUnit,
            schedule.HowOften,
            schedule.Times,
            schedule.StartsOn,
            schedule.EndsOn,
            schedule.IsActive,
            schedule.CreatedAt,
            schedule.UpdatedAt
        );

        await Send.OkAsync(response, cancellation: ct);
    }
}