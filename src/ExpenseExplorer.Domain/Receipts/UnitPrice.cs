using System.Globalization;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

/// <summary>
/// Non-negative price of one unit with at most four decimal places. More precise than
/// <see cref="Money"/> because utility bills state rates such as 7.1359 per m³.
/// </summary>
public sealed record UnitPrice
{
    public const int MaxDecimalPlaces = 4;

    private UnitPrice(decimal value) => Value = value;

    public decimal Value { get; }

    public static Result<UnitPrice> Create(decimal value) =>
        value switch
        {
            < 0m => Result.Failure<UnitPrice>(new Error("UnitPrice.Negative", "Unit price cannot be negative.")),
            _ when decimal.Round(value, MaxDecimalPlaces) != value =>
                Result.Failure<UnitPrice>(new Error(
                    "UnitPrice.TooPrecise",
                    $"Unit price cannot have more than {MaxDecimalPlaces} decimal places.")),
            _ => Result.Success(new UnitPrice(value)),
        };

    /// <summary>Price of <paramref name="quantity"/> units, rounded half away from zero to whole grosze.</summary>
    public Money Times(Quantity quantity)
    {
        ArgumentNullException.ThrowIfNull(quantity);
        return Money.RoundedFrom(Value * quantity.Value);
    }

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
