using FastEndpoints;
using Infrastructure.Domain.Auth;
using Infrastructure.Persistence;

namespace ApiService.Features.WarehouseInventory.Commands.AddMedication;

public class AddMedicationCommand(MeadDbContext dbContext, ILogger<AddMedicationCommand> logger) : Endpoint<AddMedicationRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/stock/add-medication");
        Roles(nameof(UserRole.User));
    }

    public override async Task HandleAsync(AddMedicationRequest req, CancellationToken ct)
    {
        await Send.OkAsync(ct);
    }
}