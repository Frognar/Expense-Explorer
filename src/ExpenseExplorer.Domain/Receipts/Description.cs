using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

public sealed record Description
{
    public const int MaxLength = 500;

    private Description(string value) => Value = value;

    public string Value { get; }

    public static Result<Description> Create(string? value) =>
        RequiredText.Parse(value, MaxLength, nameof(Description)).Map(text => new Description(text));

    /// <summary>Blank input means "no description" rather than an error.</summary>
    public static Result<Description?> CreateOptional(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? Result.Success<Description?>(null)
            : Create(value).Map<Description?>(description => description);

    public override string ToString() => Value;
}
