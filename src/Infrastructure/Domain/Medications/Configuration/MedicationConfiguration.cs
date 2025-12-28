using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Domain.Medications.Configuration;

public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.AlsoKnownAs)
            .HasMaxLength(200);

        builder.Property(e => e.ComesFrom)
            .HasMaxLength(100);

        builder.Property(e => e.StrengthUnit)
            .HasConversion<string>();

        builder.Property(e => e.Form)
            .HasConversion<string>();

        builder.Property(e => e.HowToTake)
            .HasConversion<string>();

        builder.HasMany(e => e.Schedules)
            .WithOne(s => s.Medication)
            .HasForeignKey(s => s.MedicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Stocks)
            .WithOne(s => s.Medication)
            .HasForeignKey(s => s.MedicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}