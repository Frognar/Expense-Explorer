using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Budget;

/// <summary>The name of a budget group, an income or a planned expense.</summary>
public sealed record BudgetName
{
    public const int MaxLength = 100;

    private BudgetName(string value) => Value = value;

    public string Value { get; }

    public static Result<BudgetName> Create(string? value) =>
        RequiredText.Parse(value, MaxLength, nameof(BudgetName)).Map(text => new BudgetName(text));

    public override string ToString() => Value;
}
