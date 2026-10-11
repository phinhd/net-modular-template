using System.ComponentModel.DataAnnotations;

namespace Users.Application.DTOs;

public sealed record RegisterRequest(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(8), MaxLength(128)] string Password,
    [Required, MaxLength(200)] string FullName);

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public sealed record AuthResponse(Guid UserId, string Email, string FullName, string Role, string Token);

public sealed record UserDto(Guid Id, string Email, string FullName, string Role, bool IsActive, DateTime CreatedAtUtc);
