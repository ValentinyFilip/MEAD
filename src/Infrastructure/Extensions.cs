using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("mead-db");

        services.AddDbContext<MeadDbContext>(options => { options.UseNpgsql(cs); });

        services.AddHealthChecks()
            .AddDbContextCheck<MeadDbContext>(
                name: "postgres-db",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "ready", "postgres"]);

        return services;
    }
}