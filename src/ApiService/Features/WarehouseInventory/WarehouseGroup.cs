using FastEndpoints;
using Infrastructure.Domain.Auth;

namespace ApiService.Features.WarehouseInventory;

public sealed class WarehouseGroup : Group
{
    public WarehouseGroup()
    {
        Configure("/warehouse/medications", ep =>
        {
            ep.Description(d => d
                .ProducesProblemDetails(401)
                .ProducesProblemDetails(403)
                .ProducesProblemDetails(500)
                .WithTags("Warehouse")
                .WithGroupName("Warehouse"));
            ep.Roles(nameof(UserRole.User), nameof(UserRole.Admin));
        });
    }
}