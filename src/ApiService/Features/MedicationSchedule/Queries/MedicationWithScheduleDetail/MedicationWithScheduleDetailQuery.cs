using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Domain.User;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.MedicationSchedule.Queries.MedicationWithScheduleDetail;

public class MedicationWithScheduleDetailQuery(MeadDbContext db)
    : Endpoint<MedicationWithScheduleDetailRequest, MedicationWithScheduleResponse>
{
    public override void Configure()
    {
        Get("/{Id:guid}");
        Group<MedicationSchedulesGroup>();
    }

    public override async Task HandleAsync(MedicationWithScheduleDetailRequest req, CancellationToken ct)
    {
        using var activity = SchedulesTelemetry.ActivitySource.StartActivity("GetMedicationSchedule");

        var userId = User.GetUserId();
        activity?.SetTag("user.id", userId);
        activity?.SetTag("schedule.id", req.Id);

        var schedule = await db.MedicationSchedules
            .AsNoTracking()
            .Include(s => s.Medication)
            .Where(s => s.Id == req.Id && s.Medication.UserId == userId)
            .SingleOrDefaultAsync(ct);

        if (schedule is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        SchedulesTelemetry.ScheduleQueriedCounter.Add(1,
            new KeyValuePair<string, object?>("user.id", userId));

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