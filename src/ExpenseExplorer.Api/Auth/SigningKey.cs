using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseExplorer.Api.Auth;

/// <summary>
/// Secret that signs access tokens. It is generated once and kept in a file, so tokens survive
/// restarts and nobody has to invent a key when installing the app.
/// </summary>
internal sealed class SigningKey
{
    private const int KeyBytes = 64;

    private SigningKey(byte[] bytes) => Key = new SymmetricSecurityKey(bytes);

    public SymmetricSecurityKey Key { get; }

    public SigningCredentials Credentials => new(Key, SecurityAlgorithms.HmacSha256);

    public static SigningKey LoadOrCreate(string path)
    {
        if (File.Exists(path))
        {
            byte[] stored = File.ReadAllBytes(path);
            return stored.Length == KeyBytes
                ? new SigningKey(stored)
                : throw new InvalidOperationException($"Signing key '{path}' is damaged. Delete it to generate a new one.");
        }

        byte[] created = RandomNumberGenerator.GetBytes(KeyBytes);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        FileStreamOptions readableOnlyByOwner = new() { Mode = FileMode.CreateNew, Access = FileAccess.Write };
        if (!OperatingSystem.IsWindows())
        {
            readableOnlyByOwner.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
        }

        using FileStream file = new(path, readableOnlyByOwner);
        file.Write(created);
        return new SigningKey(created);
    }
}
