using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Users;

/// <summary>Login of an account: 3 to 50 letters, digits, dots, dashes or underscores.</summary>
public sealed record UserName
{
    public const int MinLength = 3;
    public const int MaxLength = 50;

    private UserName(string value) => Value = value;

    public string Value { get; }

    public static Result<UserName> Create(string? value) =>
        value?.Trim() switch
        {
            null or "" => Result.Failure<UserName>(new Error("UserName.Empty", "User name is required.")),
            { Length: < MinLength or > MaxLength } => Result.Failure<UserName>(
                new Error("UserName.Length", $"User name must have {MinLength} to {MaxLength} characters.")),
            var name when !name.All(IsAllowed) => Result.Failure<UserName>(
                new Error("UserName.InvalidCharacters", "User name may contain only letters, digits, '.', '-' and '_'.")),
            var name => Result.Success(new UserName(name)),
        };

    public override string ToString() => Value;

    private static bool IsAllowed(char character) =>
        char.IsAsciiLetterOrDigit(character) || character is '.' or '-' or '_';
}
