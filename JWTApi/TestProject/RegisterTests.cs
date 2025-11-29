using JWTApi.Controllers;
using JWTApi.DTOs;
using JWTApi.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace TestProject
{
    public class RegisterTests
    {
        [Fact]
        public async Task Register_ShouldCreateUser_WhenEmailNotExists()
        {
            var db = TestHelpers.CreateDb();

            var options = TestJwt.JwtOptions();
            var config = TestJwt.Config();

            var controller = new AuthController(
                db,
                new PasswordHasher(),
                new TokenService(options),
                config
            );

            var req = new RegisterRequest
            {
                Email = "user@test.com",
                Password = "test123",
                ConfirmPassword = "test123"
            };

            var result = await controller.Register(req);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
