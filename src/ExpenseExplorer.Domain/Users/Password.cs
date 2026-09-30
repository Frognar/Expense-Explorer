using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Users;

/// <summary>
/// A new password. Length is the only rule: long passphrases are both easier to remember
/// and harder to guess than short ones with forced symbols.
/// </summary>
public sealed class Password
{
    public const int MinLength = 10;
    public const int MaxLength = 128;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Result<Password> Create(string? value) =>
        value switch
        {
            null or { Length: < MinLength } => Result.Failure<Password>(
                new Error("Password.TooShort", $"Password must have at least {MinLength} characters.")),
            { Length: > MaxLength } => Result.Failure<Password>(
                new Error("Password.TooLong", $"Password cannot be longer than {MaxLength} characters.")),
            _ => Result.Success(new Password(value)),
        };

    /// <summary>Keeps the password out of logs and debugger views.</summary>
    public override string ToString() => "***";
}
