using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Domain.Medications.Configuration;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Unit)
            .HasConversion<string>();

        builder.Property(e => e.WhereItsStored)
            .HasMaxLength(100);

        builder.Property(e => e.BatchNumber)
            .HasMaxLength(50);
    }
}