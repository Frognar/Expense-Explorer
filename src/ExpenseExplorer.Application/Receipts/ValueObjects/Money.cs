namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Money
{
    public decimal Value { get; }

    private Money(decimal value) => Value = value;

    public static Result<Money, string> TryCreate(decimal value)
    {
        return value < decimal.Zero
            ? Result.Failure<Money, string>("Money cannot be negative")
            : Result.Success<Money, string>(new Money(value));
    }
}