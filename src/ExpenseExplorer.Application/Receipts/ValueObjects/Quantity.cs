using DotMaybe;

namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Quantity
{
    public decimal Value { get; }

    private Quantity(decimal value) => Value = value;

    public static Maybe<Quantity> TryCreate(decimal value) =>
        value < decimal.Zero
            ? None.OfType<Quantity>()
            : Some.With(new Quantity(value));
}