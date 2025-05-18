namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct PurchaseDate
{
    public DateOnly Date { get; }

    private PurchaseDate(DateOnly date) => Date = date;

    public static Result<PurchaseDate, string> TryCreate(DateOnly date, DateOnly today)
    {
        return date > today
            ? Result.Failure<PurchaseDate, string>("Purchase date cannot be in the future")
            : Result.Success<PurchaseDate, string>(new PurchaseDate(date));
    }
}