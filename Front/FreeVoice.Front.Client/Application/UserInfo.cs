using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FreeVoice.Front.Client.Application;

public sealed class UserInfo
{
    public required string UserId { get; init; }
    public required string Name { get; init; }
    public string? Email { get; init; }
    
    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal)
    {
        return new UserInfo
        {
            UserId = GetRequiredClaim(principal, JwtRegisteredClaimNames.Sub),
            Name = GetRequiredClaim(principal, JwtRegisteredClaimNames.Name),
            Email = GetOptionalClaim(principal, JwtRegisteredClaimNames.Email),
        };
    }

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var baseClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, UserId),
            new(JwtRegisteredClaimNames.Name, Name),
            new(JwtRegisteredClaimNames.Email, Email ?? string.Empty),
        };

        var identity = new ClaimsIdentity(
            authenticationType: nameof(UserInfo),
            nameType: JwtRegisteredClaimNames.Name,
            roleType: ClaimTypes.Role
        );
        
        return new ClaimsPrincipal(identity);
    }

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value ??
        throw new InvalidOperationException($"Could not find required '{claimType}' claim.");

    private static string? GetOptionalClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value;

    private static List<string> GetRoles(ClaimsPrincipal principal, string claimType) =>
        principal.FindAll(claimType).Select(claim => claim.Value).ToList();
}
