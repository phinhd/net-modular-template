using System.ComponentModel.DataAnnotations;

namespace Users.Infrastructure.Auth;

/// <summary>
/// Bound from the "Jwt" configuration section and validated on startup,
/// so a missing or weak signing key fails fast instead of at the first login.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = default!;

    [Required]
    public string Audience { get; set; } = default!;

    /// <summary>HMAC-SHA256 key. Must be at least 32 characters (256 bits).</summary>
    [Required, MinLength(32)]
    public string SigningKey { get; set; } = default!;

    [Range(1, 24 * 60)]
    public int ExpiryMinutes { get; set; } = 60;
}
