using DotMaybe;

namespace ExpenseExplorer.Application.Domain.ValueObjects;

public readonly record struct PurchaseDate
{
    public DateOnly Date { get; }

    private PurchaseDate(DateOnly date) => Date = date;

    public static Maybe<PurchaseDate> TryCreate(DateOnly date, DateOnly today) =>
        date > today
            ? None.OfType<PurchaseDate>()
            : Some.With(new PurchaseDate(date));
}