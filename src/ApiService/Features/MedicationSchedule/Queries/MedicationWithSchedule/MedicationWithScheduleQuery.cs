using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Domain.User;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.MedicationSchedule.Queries.MedicationWithSchedule;

public class MedicationWithScheduleQuery(MeadDbContext db)
    : Endpoint<EmptyRequest, MedicationWithSchedulesResponse>
{
    public override void Configure()
    {
        Get("");
        Group<MedicationSchedulesGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        using var activity = SchedulesTelemetry.ActivitySource.StartActivity("GetMedicationSchedules");

        var userId = User.GetUserId();
        activity?.SetTag("user.id", userId);

        // Verify medication belongs to user
        var medicationExists = await db.Medications
            .AsNoTracking()
            .AnyAsync(m => m.UserId == userId, ct);

        if (!medicationExists)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var now = DateTime.UtcNow;

        var schedules = await db.MedicationSchedules
            .AsNoTracking()
            .Include(s => s.Medication)
            .Where(s => s.Medication.UserId == userId)
            .OrderByDescending(s => s.IsActive)
            .ThenBy(s => s.StartsOn)
            .Select(s => new MedicationWithScheduleItem(
                s.Id,
                s.MedicationId,
                s.Medication.Name,
                s.AmountPerTime,
                s.AmountUnit,
                s.HowOften,
                s.Times,
                s.StartsOn,
                s.EndsOn,
                s.IsActive,
                s.EndsOn.HasValue && s.EndsOn.Value < now
            ))
            .ToListAsync(ct);

        SchedulesTelemetry.ScheduleListQueriedCounter.Add(1,
            new KeyValuePair<string, object?>("user.id", userId),
            new KeyValuePair<string, object?>("result.count", schedules.Count));

        var response = new MedicationWithSchedulesResponse(schedules);

        await Send.OkAsync(response, cancellation: ct);
    }
}