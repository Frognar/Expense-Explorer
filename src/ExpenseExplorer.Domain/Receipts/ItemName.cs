using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

public sealed record ItemName
{
    public const int MaxLength = 100;

    private ItemName(string value) => Value = value;

    public string Value { get; }

    public static Result<ItemName> Create(string? value) =>
        RequiredText.Parse(value, MaxLength, "ItemName").Map(text => new ItemName(text));

    public override string ToString() => Value;
}
