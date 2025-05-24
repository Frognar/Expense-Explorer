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

    public static readonly Money Zero = new(0);

    public static Money operator +(Money left, Money right) => Add(left, right);

    public static Money Add(Money left, Money right) => new(left.Value + right.Value);

    public static Money operator *(Money left, Quantity right) => Multiply(left, right);

    public static Money Multiply(Money left, Quantity right) => new(left.Value * right.Value);

    public static Money operator -(Money left, Money right) => Subtract(left, right);

    public static Money Subtract(Money left, Money right) =>
        left.Value < right.Value ? Zero : new Money(left.Value - right.Value);
}