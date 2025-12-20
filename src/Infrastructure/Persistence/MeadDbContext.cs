using Infrastructure.Domain.Auth;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}