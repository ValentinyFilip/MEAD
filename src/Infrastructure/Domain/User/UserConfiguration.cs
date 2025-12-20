using Infrastructure.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Domain.User;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        var rolesConverter = new ValueConverter<List<UserRole>, string>(
            v => string.Join(',', v.Select(r => r.ToString())),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(Enum.Parse<UserRole>)
                .ToList());

        b.HasKey(x => x.Id);

        b.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);

        b.HasIndex(x => x.Email).IsUnique();

        b.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(128);

        b.Property(x => x.Roles)
            .HasConversion(rolesConverter)
            .HasMaxLength(256);
    }
}