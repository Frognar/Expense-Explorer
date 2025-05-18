namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Quantity
{
    public decimal Value { get; }

    private Quantity(decimal value) => Value = value;

    public static Result<Quantity, string> TryCreate(decimal value)
    {
        return value < decimal.Zero
            ? Result.Failure<Quantity, string>("Quantity cannot be negative")
            : Result.Success<Quantity, string>(new Quantity(value));
    }
}