using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuildingBlocks.Application.Exceptions;

namespace WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Reads the user id from the "sub" claim of the validated JWT.</summary>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new AuthenticationFailedException("The access token does not contain a valid subject.");
    }
}
