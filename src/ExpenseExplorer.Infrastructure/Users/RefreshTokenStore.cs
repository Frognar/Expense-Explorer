using System.Security.Cryptography;
using System.Text;
using ExpenseExplorer.Application.Users;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Users;

internal sealed class RefreshTokenStore(ExpenseExplorerDbContext db) : IRefreshTokens
{
    private const int TokenBytes = 32;

    public async Task<RefreshToken> IssueAsync(Guid userId, DateTimeOffset now, TimeSpan lifetime, CancellationToken cancellationToken)
    {
        await db.RefreshTokens
            .Where(row => row.UserId == userId && row.ExpiresAt < now)
            .ExecuteDeleteAsync(cancellationToken);

        RefreshToken token = new(Base64Url(RandomNumberGenerator.GetBytes(TokenBytes)), now + lifetime);
        db.RefreshTokens.Add(new RefreshTokenRow
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            TokenHash = Hash(token.Value),
            CreatedAt = now,
            ExpiresAt = token.ExpiresAt,
        });
        await db.SaveChangesAsync(cancellationToken);
        return token;
    }

    public async Task<Guid?> RedeemAsync(string token, DateTimeOffset now, CancellationToken cancellationToken)
    {
        RefreshTokenRow? row = await FindAsync(token, cancellationToken);
        switch (row)
        {
            case null:
                return null;
            case { RevokedAt: not null }:
                await RevokeAllAsync(row.UserId, now, cancellationToken);
                return null;
            case var expired when expired.ExpiresAt <= now:
                return null;
            default:
                row.RevokedAt = now;
                await db.SaveChangesAsync(cancellationToken);
                return row.UserId;
        }
    }

    public async Task RevokeAsync(string token, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (await FindAsync(token, cancellationToken) is { RevokedAt: null } row)
        {
            row.RevokedAt = now;
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    public Task RevokeAllAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken) =>
        db.RefreshTokens
            .Where(row => row.UserId == userId && row.RevokedAt == null)
            .ExecuteUpdateAsync(set => set.SetProperty(row => row.RevokedAt, now), cancellationToken);

    private Task<RefreshTokenRow?> FindAsync(string token, CancellationToken cancellationToken)
    {
        string hash = Hash(token);
        return db.RefreshTokens.SingleOrDefaultAsync(row => row.TokenHash == hash, cancellationToken);
    }

    private static string Hash(string token) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
