using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.Auth;

public sealed record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required, MaxLength(128)] string DisplayName);

public sealed record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);

public sealed record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, string UserId, string Email, string DisplayName);
