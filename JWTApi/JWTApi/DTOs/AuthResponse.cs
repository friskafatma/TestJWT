namespace JWTApi.DTOs;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }
}
