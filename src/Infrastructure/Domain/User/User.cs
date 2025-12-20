using Infrastructure.Domain.Auth;

namespace Infrastructure.Domain.User;

public class User
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public RefreshToken? RefreshToken { get; set; }
    public List<UserRole> Roles { get; set; } = [];
}