using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MigrationService;

public class MigrationWorker(IServiceProvider serviceProvider, ILogger<MigrationWorker> logger, IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MeadDbContext>();

            logger.LogInformation("Applying migrations...");
            await db.Database.MigrateAsync(ct);

            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying migrations.");
            Environment.ExitCode = -1;
        }
        finally
        {
            logger.LogInformation("Migration worker completed. Shutting down.");
            lifetime.StopApplication();
        }
    }
}