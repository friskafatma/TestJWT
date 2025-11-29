using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using JWTApi.Models;

namespace TestProject
{
    public static class TestJwt
    {
        public static IConfiguration Config()
        {
            var settings = new Dictionary<string, string>
            {
                {"Jwt:Key", "SUPER_SECRET_KEY_CHANGE_THIS_123456789"},
                {"Jwt:Issuer", "FriskaJWTAuth"},
                {"Jwt:Audience", "FriskaJWTAuth"},
                {"Jwt:AccessTokenMinutes", "15"},
                {"Jwt:RefreshTokenDays", "7"},
                {"Jwt:IdleTimeoutMinutes", "15"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        public static IOptions<JWTOptions> JwtOptions()
        {
            var cfg = Config();

            return Options.Create(new JWTOptions
            {
                Key = cfg["Jwt:Key"],
                Issuer = cfg["Jwt:Issuer"],
                Audience = cfg["Jwt:Audience"],
                AccessTokenMinutes = int.Parse(cfg["Jwt:AccessTokenMinutes"]),
                RefreshTokenDays = int.Parse(cfg["Jwt:RefreshTokenDays"]),
                IdleTimeoutMinutes = int.Parse(cfg["Jwt:IdleTimeoutMinutes"]),
            });
        }
    }
}
