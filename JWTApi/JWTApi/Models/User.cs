using System.ComponentModel.DataAnnotations;

namespace JWTApi.Models;

public class User
{
    public Guid Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [System.Text.Json.Serialization.JsonIgnore]
    public string PasswordHash { get; set; } = null!;


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<RefreshToken> RefreshTokens { get; set; } = new();
}

