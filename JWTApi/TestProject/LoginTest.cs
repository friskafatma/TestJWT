using JWTApi.Controllers;
using JWTApi.Models;
using JWTApi.Services;
using JWTApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class LoginTest
    {
        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsCorrect()
        {
            var db = TestHelpers.CreateDb();
            var options = TestJwt.JwtOptions();
            var config = TestJwt.Config();
            var hasher = new PasswordHasher();

            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "user@test.com",
                PasswordHash = hasher.Hash("test123"),
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();

            var controller = new AuthController(
                db,
                hasher,
                new TokenService(options),
                config
            );

            var req = new LoginRequest
            {
                Email = "user@test.com",
                Password = "test123"
            };

            var result = await controller.Login(req);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

    }
}


