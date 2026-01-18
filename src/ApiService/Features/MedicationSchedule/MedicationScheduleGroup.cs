using FastEndpoints;
using Infrastructure.Domain.Auth;

namespace ApiService.Features.MedicationSchedule;

public sealed class MedicationSchedulesGroup : Group
{
    public MedicationSchedulesGroup()
    {
        Configure("/medications/schedules", ep =>
        {
            ep.Description(d => d
                .ProducesProblemDetails(401)
                .ProducesProblemDetails(403)
                .ProducesProblemDetails(500)
                .WithTags("Medication Schedules")
                .WithGroupName("Medication Schedules"));
            ep.Roles(nameof(UserRole.User), nameof(UserRole.Admin));
        });
    }
}