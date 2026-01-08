using FastEndpoints;
using Infrastructure.Domain.Auth;
using Infrastructure.Persistence;

namespace ApiService.Features.MedicationSchedule.Commands.AddMedicationWithSchedule;

public class AddMedicationWithScheduleCommand(MeadDbContext dbContext) : Endpoint<AddMedicationWithScheduleRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/schedule/add-medication");
        Roles(nameof(UserRole.User));
    }

    public async override Task HandleAsync(AddMedicationWithScheduleRequest req, CancellationToken ct)
    {
        await Send.OkAsync(ct);
    }
}