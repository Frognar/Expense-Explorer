using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Domain.Budget;

/// <summary>Money coming into a period (positive) or taken out of it, e.g. savings or unpaid leave (negative).</summary>
public sealed record FundAmount
{
    private FundAmount(decimal value) => Value = value;

    public decimal Value { get; }

    public static Result<FundAmount> Create(decimal value) =>
        value switch
        {
            0m => Result.Failure<FundAmount>(new Error("FundAmount.Zero", "The amount cannot be zero.")),
            _ when decimal.Round(value, Money.MaxDecimalPlaces) != value => Result.Failure<FundAmount>(new Error(
                "Money.TooPrecise", $"Amount cannot have more than {Money.MaxDecimalPlaces} decimal places.")),
            _ => Result.Success(new FundAmount(value)),
        };
}
