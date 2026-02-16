using Infrastructure.Domain.Auth;
using Infrastructure.Domain.Medications;
using Infrastructure.Domain.Medications.Configuration;
using Infrastructure.Domain.Notifications;
using Infrastructure.Domain.Notifications.Configuration;
using Infrastructure.Domain.User;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class MeadDbContext : DbContext
{
    public MeadDbContext(DbContextOptions<MeadDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<MedicationSchedule> MedicationSchedules { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new MedicationConfiguration());
        modelBuilder.ApplyConfiguration(new MedicationScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new StockConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
    }
}