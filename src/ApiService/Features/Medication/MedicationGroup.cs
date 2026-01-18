using FastEndpoints;

namespace ApiService.Features.Medication;

public sealed class MedicationsGroup : Group
{
    public MedicationsGroup()
    {
        Configure("/medications", ep =>
        {
            ep.Description(d => d
                .ProducesProblemDetails(401)
                .ProducesProblemDetails(403)
                .ProducesProblemDetails(404)
                .ProducesProblemDetails(500)
                .WithTags("Medications")
                .WithGroupName("Medications"));
        });
    }
}