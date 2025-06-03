namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Quantity
{
    public decimal Value { get; }

    private Quantity(decimal value) => Value = value;

    public static Option<Quantity> TryCreate(decimal value) =>
        value < decimal.Zero
            ? Option.None<Quantity>()
            : Option.Some(new Quantity(value));
}