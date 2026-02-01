using ApiService.Common.Extensions;
using ApiService.Features.MedicationSchedule.Queries.MedicationWithScheduleDetail;
using FastEndpoints;
using Infrastructure.Domain.User;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.MedicationSchedule.Commands.AddMedicationWithSchedule;

public class AddMedicationWithScheduleCommand(MeadDbContext db)
    : Endpoint<AddMedicationWithScheduleRequest, MedicationWithScheduleResponse>
{
    public override void Configure()
    {
        Post("");
        Group<MedicationSchedulesGroup>();
    }

    public override async Task HandleAsync(AddMedicationWithScheduleRequest req, CancellationToken ct)
    {
        using var activity = SchedulesTelemetry.ActivitySource.StartActivity("AddMedicationSchedule");
        
        var userId = User.GetUserId();
        activity?.SetTag("user.id", userId);
        activity?.SetTag("medication.id", req.MedicationId);

        var medication = await db.Medications
            .AsNoTracking()
            .Where(m => m.Id == req.MedicationId && m.UserId == userId)
            .SingleOrDefaultAsync(ct);

        if (medication is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var schedule = new Infrastructure.Domain.Medications.MedicationSchedule
        {
            MedicationId = req.MedicationId,
            AmountPerTime = req.AmountPerTime,
            AmountUnit = req.AmountUnit,
            HowOften = req.HowOften,
            Times = req.Times,
            StartsOn = req.StartsOn.ToUniversalTime(),
            EndsOn = req.EndsOn?.ToUniversalTime(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        db.MedicationSchedules.Add(schedule);
        await db.SaveChangesAsync(ct);

        SchedulesTelemetry.ScheduleCreatedCounter.Add(1,
            new KeyValuePair<string, object?>("user.id", userId),
            new KeyValuePair<string, object?>("medication.id", req.MedicationId),
            new KeyValuePair<string, object?>("frequency", req.HowOften.ToString()));
        SchedulesTelemetry.TotalActiveSchedulesCounter.Add(1);
        SchedulesTelemetry.DailyDosesHistogram.Record(req.Times.Count);

        if (req.EndsOn.HasValue)
        {
            var durationDays = (req.EndsOn.Value - req.StartsOn).TotalDays;
            SchedulesTelemetry.ScheduleDurationDaysHistogram.Record(durationDays);
        }

        var response = new MedicationWithScheduleResponse(
            schedule.Id,
            medication.Id,
            medication.Name,
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

        await Send.CreatedAtAsync<MedicationWithScheduleDetailQuery>(
            new { Id = schedule.Id },
            response,
            cancellation: ct);
    }
}