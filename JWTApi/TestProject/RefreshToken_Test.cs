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
    public class RefreshToken_Test
    {
        [Fact]
        public async Task Refresh_ShouldFail_WhenIdleMoreThan15Minutes()
        {
            var db = TestHelpers.CreateDb();
            var options = TestJwt.JwtOptions();
            var config = TestJwt.Config();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@test.com",
                PasswordHash = "xxx",
                CreatedAt = DateTime.UtcNow
            };


            db.Users.Add(user);

            db.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = "oldtoken",
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                LastActivityAt = DateTime.UtcNow.AddMinutes(-20)
            });





            db.SaveChanges();

            var controller = new AuthController(
                db,
                new PasswordHasher(),
                new TokenService(options),
                config
            );

            var req = new RefreshRequest { RefreshToken = "oldtoken" };

            var result = await controller.Refresh(req);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

    }
}
