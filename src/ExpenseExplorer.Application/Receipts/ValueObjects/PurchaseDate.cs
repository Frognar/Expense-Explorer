namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct PurchaseDate
{
    public DateOnly Date { get; }

    private PurchaseDate(DateOnly date) => Date = date;

    public static Option<PurchaseDate> TryCreate(DateOnly date, DateOnly today) =>
        date > today
            ? Option.None<PurchaseDate>()
            : Option.Some(new PurchaseDate(date));
}