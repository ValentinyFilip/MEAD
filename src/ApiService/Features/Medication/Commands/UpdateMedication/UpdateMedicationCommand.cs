using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.Medication.Commands.UpdateMedication;

public sealed class UpdateMedication(MeadDbContext db)
    : Endpoint<UpdateMedicationRequest, MedicationResponse>
{
    public override void Configure()
    {
        Put("{id}");
        Group<MedicationsGroup>();
    }

    public override async Task HandleAsync(UpdateMedicationRequest req, CancellationToken ct)
    {
        var userId = User.GetUserId();

        var medication = await db.Medications
            .Where(m => m.Id == req.Id && m.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (medication is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        medication.Name = req.Name;
        medication.AlsoKnownAs = req.AlsoKnownAs;
        medication.Form = req.Form;
        medication.Strength = req.Strength;
        medication.StrengthUnit = req.StrengthUnit;
        medication.HowToTake = req.HowToTake;
        medication.ComesFrom = req.ComesFrom;
        medication.IsActive = req.IsActive;
        medication.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        var response = new MedicationResponse(
            medication.Id,
            medication.Name,
            medication.AlsoKnownAs,
            medication.Form,
            medication.Strength,
            medication.StrengthUnit,
            medication.HowToTake,
            medication.ComesFrom,
            medication.IsActive,
            medication.CreatedAt
        );

        await Send.OkAsync(response, ct);
    }
}