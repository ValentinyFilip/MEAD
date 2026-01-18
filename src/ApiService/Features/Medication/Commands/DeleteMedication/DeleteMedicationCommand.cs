using ApiService.Common.Extensions;
using FastEndpoints;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Features.Medication.Commands.DeleteMedication;

public sealed class DeleteMedication(MeadDbContext db)
    : Endpoint<DeleteMedicationRequest>
{
    public override void Configure()
    {
        Delete("{id}");
        Group<MedicationsGroup>();
    }

    public override async Task HandleAsync(DeleteMedicationRequest req, CancellationToken ct)
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

        db.Medications.Remove(medication);
        await db.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}