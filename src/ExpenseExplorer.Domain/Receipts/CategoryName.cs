using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

public sealed record CategoryName
{
    public const int MaxLength = 100;

    private CategoryName(string value) => Value = value;

    public string Value { get; }

    public static Result<CategoryName> Create(string? value) =>
        RequiredText.Parse(value, MaxLength, "CategoryName").Map(text => new CategoryName(text));

    public override string ToString() => Value;
}
