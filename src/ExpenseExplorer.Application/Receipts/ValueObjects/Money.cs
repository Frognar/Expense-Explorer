using DotMaybe;

namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Money
{
    public decimal Value { get; }

    private Money(decimal value) => Value = value;

    public static Maybe<Money> TryCreate(decimal value) =>
        value < decimal.Zero
            ? None.OfType<Money>()
            : Some.With(new Money(value));

    public static readonly Money Zero = new(0);

    public static Money operator +(Money left, Money right) => Add(left, right);

    public static Money Add(Money left, Money right) => new(left.Value + right.Value);

    public static Money operator *(Money left, Quantity right) => Multiply(left, right);

    public static Money Multiply(Money left, Quantity right) => new(left.Value * right.Value);

    public static Money operator -(Money left, Money right) => Subtract(left, right);

    public static Money Subtract(Money left, Money right) =>
        left.Value < right.Value ? Zero : new Money(left.Value - right.Value);

    public static bool operator <(Money left, Money right) => left.Value < right.Value;

    public static bool operator >(Money left, Money right) => left.Value > right.Value;

    public int CompareTo(Money other) => Value.CompareTo(other.Value);
}