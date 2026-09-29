namespace ExpenseExplorer.Domain.Common;

internal static class RequiredText
{
    public static Result<string> Parse(string? value, int maxLength, string errorPrefix) =>
        value?.Trim() switch
        {
            null or "" => Result.Failure<string>(
                new Error($"{errorPrefix}.Empty", "Value is required.")),
            { Length: var length } when length > maxLength => Result.Failure<string>(
                new Error($"{errorPrefix}.TooLong", $"Value cannot be longer than {maxLength} characters.")),
            var trimmed => Result.Success(trimmed),
        };
}
