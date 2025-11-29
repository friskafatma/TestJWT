namespace JWTApi.Models
{
    public class JWTOptions
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int AccessTokenMinutes { get; set; }
        public int RefreshTokenDays { get; set; }

        public int IdleTimeoutMinutes { get; set; }
    }
}
