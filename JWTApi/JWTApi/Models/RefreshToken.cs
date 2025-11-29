using JWTApi.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

}
