using System.ComponentModel.DataAnnotations;

namespace JWTApi.DTOs;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = null!;
}

