using System.Globalization;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

/// <summary>Non-negative amount with at most two decimal places.</summary>
public sealed record Money : IComparable<Money>
{
    public const int MaxDecimalPlaces = 2;

    public static readonly Money Zero = new(0m);

    private Money(decimal value) => Value = value;

    public decimal Value { get; }

    public static Result<Money> Create(decimal value) =>
        value switch
        {
            < 0m => Result.Failure<Money>(new Error("Money.Negative", "Amount cannot be negative.")),
            _ when decimal.Round(value, MaxDecimalPlaces) != value =>
                Result.Failure<Money>(new Error(
                    "Money.TooPrecise",
                    $"Amount cannot have more than {MaxDecimalPlaces} decimal places.")),
            _ => Result.Success(new Money(value)),
        };

    public static Money Sum(IEnumerable<Money> amounts) =>
        new(amounts.Sum(amount => amount.Value));

    public static bool operator <(Money left, Money right) => left.Value < right.Value;

    public static bool operator >(Money left, Money right) => left.Value > right.Value;

    public static bool operator <=(Money left, Money right) => left.Value <= right.Value;

    public static bool operator >=(Money left, Money right) => left.Value >= right.Value;

    public int CompareTo(Money? other) => other is null ? 1 : Value.CompareTo(other.Value);

    /// <summary>Price of <paramref name="quantity"/> units, rounded half away from zero to whole grosze.</summary>
    public Money Times(Quantity quantity) =>
        new(decimal.Round(Value * quantity.Value, MaxDecimalPlaces, MidpointRounding.AwayFromZero));

    /// <summary>Difference clamped at zero, so the result is always a valid amount.</summary>
    public Money MinusClamped(Money other) => other > this ? Zero : new Money(Value - other.Value);

    public override string ToString() => Value.ToString("0.00", CultureInfo.InvariantCulture);
}
