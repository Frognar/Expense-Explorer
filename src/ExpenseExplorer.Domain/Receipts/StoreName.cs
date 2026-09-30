using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

public sealed record StoreName
{
    public const int MaxLength = 100;

    private StoreName(string value) => Value = value;

    public string Value { get; }

    public static Result<StoreName> Create(string? value) =>
        RequiredText.Parse(value, MaxLength, "StoreName").Map(text => new StoreName(text));

    public override string ToString() => Value;
}
