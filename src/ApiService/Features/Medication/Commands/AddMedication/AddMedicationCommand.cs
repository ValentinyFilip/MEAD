using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Persistence;

namespace ApiService.Features.Medication.Commands.AddMedication;

public sealed class AddMedication(MeadDbContext db)
    : Endpoint<AddMedicationRequest, MedicationResponse>
{
    public override void Configure()
    {
        Post("");
        Group<MedicationsGroup>();
    }

    public override async Task HandleAsync(AddMedicationRequest req, CancellationToken ct)
    {
        var userId = User.GetUserId();

        var medication = new Infrastructure.Domain.Medications.Medication
        {
            UserId = userId,
            Name = req.Name,
            AlsoKnownAs = req.AlsoKnownAs,
            Form = req.Form,
            Strength = req.Strength,
            StrengthUnit = req.StrengthUnit,
            HowToTake = req.HowToTake,
            ComesFrom = req.ComesFrom,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        db.Medications.Add(medication);
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

        await Send.OkAsync(
            response,
            cancellation: ct);
    }
}