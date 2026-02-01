using System.Diagnostics;
using ApiService.Common.Extensions;
using ApiService.Features.Medication.Commands;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.Medication.Queries.List;

public sealed class MedicationsListQuery(
    MeadDbContext db,
    ILogger<MedicationsListQuery> logger)
    : EndpointWithoutRequest<List<MedicationResponse>>
{
    public override void Configure()
    {
        Get("/");
        Group<MedicationsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var activity = MedicationsTelemetry.ActivitySource.StartActivity("GetMedicationsList");
        var userId = User.GetUserId();

        if (activity?.IsAllDataRequested == true)
        {
            activity.SetTag("user.id", userId);
            activity.SetTag("operation.name", "list_medications");
        }

        logger.LogInformation("Fetching medications list for user {UserId}", userId);

        try
        {
            var medications = await db.Medications
                .AsNoTracking()
                .Where(m => m.UserId == userId && m.IsActive)
                .OrderBy(m => m.Name)
                .Select(m => new MedicationResponse(
                    m.Id,
                    m.Name,
                    m.AlsoKnownAs,
                    m.Form,
                    m.Strength,
                    m.StrengthUnit,
                    m.HowToTake,
                    m.ComesFrom,
                    m.IsActive,
                    m.CreatedAt
                ))
                .ToListAsync(ct);

            if (activity?.IsAllDataRequested == true)
            {
                activity.SetTag("result.count", medications.Count);
            }

            logger.LogInformation(
                "Retrieved {MedicationCount} medications for user {UserId}",
                medications.Count,
                userId);

            // Record metrics
            MedicationsTelemetry.MedicationListQueriedCounter.Add(
                1,
                new KeyValuePair<string, object?>("user.id", userId));

            await Send.OkAsync(medications, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching medications list for user {UserId}", userId);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}