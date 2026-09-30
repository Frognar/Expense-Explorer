using System.Globalization;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

/// <summary>Positive amount of units (pieces, kilograms, litres) with at most three decimal places.</summary>
public sealed record Quantity
{
    public const int MaxDecimalPlaces = 3;

    public static readonly Quantity One = new(1m);

    private Quantity(decimal value) => Value = value;

    public decimal Value { get; }

    public static Result<Quantity> Create(decimal value) =>
        value switch
        {
            <= 0m => Result.Failure<Quantity>(new Error("Quantity.NotPositive", "Quantity must be greater than zero.")),
            _ when decimal.Round(value, MaxDecimalPlaces) != value =>
                Result.Failure<Quantity>(new Error(
                    "Quantity.TooPrecise",
                    $"Quantity cannot have more than {MaxDecimalPlaces} decimal places.")),
            _ => Result.Success(new Quantity(value)),
        };

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
