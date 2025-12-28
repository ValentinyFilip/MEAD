using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Domain.Medications.Configuration;

public class MedicationScheduleConfiguration : IEntityTypeConfiguration<MedicationSchedule>
{
    public void Configure(EntityTypeBuilder<MedicationSchedule> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.AmountUnit)
            .HasConversion<string>();

        builder.Property(e => e.HowOften)
            .HasConversion<string>();

        builder.Property(e => e.Times)
            .HasColumnType("jsonb");
    }
}