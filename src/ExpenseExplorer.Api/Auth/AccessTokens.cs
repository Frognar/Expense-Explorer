using System.Security.Claims;
using ExpenseExplorer.Application.Users;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseExplorer.Api.Auth;

internal sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);

/// <summary>Short-lived JWTs that carry who the user is and what they may do.</summary>
internal static class AccessTokens
{
    public const string UserIdClaim = JwtRegisteredClaimNames.Sub;
    public const string NameClaim = JwtRegisteredClaimNames.Name;
    public const string RoleClaim = "role";

    public static AccessToken Create(AuthenticatedUser user, DateTimeOffset now, AuthOptions options, SigningKey key)
    {
        DateTimeOffset expiresAt = now + options.AccessTokenLifetime;
        string token = new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false }.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = options.Issuer,
            Audience = options.Audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = key.Credentials,
            Subject = new ClaimsIdentity(
            [
                new Claim(UserIdClaim, user.Id.ToString()),
                new Claim(NameClaim, user.UserName),
                new Claim(RoleClaim, user.Role.ToString()),
            ]),
        });

        return new AccessToken(token, expiresAt);
    }

    /// <summary>Lifetime is checked against the app's clock, the same one that issued the token.</summary>
    public static TokenValidationParameters ValidationParameters(AuthOptions options, SigningKey key, TimeProvider clock) =>
        new()
        {
            ValidIssuer = options.Issuer,
            ValidAudience = options.Audience,
            IssuerSigningKey = key.Key,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            NameClaimType = NameClaim,
            RoleClaimType = RoleClaim,
            LifetimeValidator = (notBefore, expires, _, _) => IsWithin(notBefore, expires, clock.GetUtcNow().UtcDateTime),
        };

    private static bool IsWithin(DateTime? notBefore, DateTime? expires, DateTime now) =>
        expires is { } end && now < end && (notBefore is not { } start || start <= now);
}
