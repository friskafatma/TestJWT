using JWTApi.Data;
using JWTApi.DTOs;
using JWTApi.Models;
using JWTApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace JWTApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _token;
    private readonly IConfiguration _config;


    public AuthController(AppDbContext db, IPasswordHasher hasher, ITokenService token, IConfiguration config)
    {
        _db = db;
        _hasher = hasher;
        _token = token;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {

        if (await _db.Users.AnyAsync(x => x.Email == req.Email))
            return BadRequest("Email already registered");

        var user = new User
        {
            Email = req.Email,
            PasswordHash = _hasher.Hash(req.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok("Register success");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == req.Email);

        if (user == null)
            return Unauthorized("Invalid email or password");

        if (!_hasher.Verify(user.PasswordHash, req.Password))
            return Unauthorized("Invalid email or password");


        var accessToken = _token.GenerateAccess(user, out var expires);


        var refreshToken = _token.GenerateRefresh(user);

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            Message = $"Hello {user.Email}, welcome back!",
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            UserId = user.Id,
            Email = user.Email,
            AccessTokenExpiresAt = expires
        });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userIdString = User.Claims
            .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized(new { message = "Invalid token" });

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Unauthorized(new { message = "User not found" });

        return Ok(user);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest req)
    {
        var oldToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == req.RefreshToken);

        if (oldToken == null)
            return Unauthorized(new { message = "Invalid refresh token" });

        if (oldToken.RevokedAt != null)
            return Unauthorized(new { message = "Refresh token already revoked" });

        if (oldToken.ExpiresAt < DateTime.UtcNow)
            return Unauthorized(new { message = "Refresh token expired" });


        var idleMinutes = int.Parse(_config["Jwt:IdleTimeoutMinutes"] ?? "15");
        var idleLimit = TimeSpan.FromMinutes(idleMinutes);

        if (DateTime.UtcNow - oldToken.LastActivityAt > idleLimit)
        {
            oldToken.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Unauthorized(new
            {
                message = "Session expired due to inactivity. Please login again."
            });
        }

        oldToken.LastActivityAt = DateTime.UtcNow;


        var newRefreshToken = _token.GenerateRefresh(oldToken.User);

        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.ReplacedByToken = newRefreshToken.Token;

        _db.RefreshTokens.Add(newRefreshToken);

        var newAccessToken = _token.GenerateAccess(oldToken.User, out var accessExpires);

        await _db.SaveChangesAsync();

        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            UserId = oldToken.User.Id,
            Email = oldToken.User.Email,
            AccessTokenExpiresAt = accessExpires
        });
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest req)
    {
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == req.RefreshToken);

        if (token == null)
            return NotFound(new { message = "Refresh token not found" });


        token.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Logged out successfully" });
    }
}
