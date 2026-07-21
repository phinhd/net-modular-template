using Users.Domain.Entities;

namespace Users.Domain.Interfaces;

/// <summary>
/// Implemented in Users.Infrastructure/Auth/JwtTokenService.cs, since generating
/// a token requires config (signing key, issuer) that belongs to Infrastructure.
/// </summary>
public interface IJwtTokenService
{
    string GenerateToken(User user);
}
