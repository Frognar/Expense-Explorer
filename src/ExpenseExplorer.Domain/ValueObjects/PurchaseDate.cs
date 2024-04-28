namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public record PurchaseDate
{
  public static readonly PurchaseDate MinValue = new(DateOnly.MinValue, DateOnly.MinValue);

  private PurchaseDate(DateOnly date, DateOnly today)
  {
    FutureDateException.ThrowIfFutureDate(date, today);
    Date = date;
  }

  public DateOnly Date { get; }

  public static PurchaseDate Create(DateOnly date, DateOnly today)
  {
    return new PurchaseDate(date, today);
  }

  public static Maybe<PurchaseDate> TryCreate(DateOnly? date, DateOnly today)
  {
    return !date.HasValue || date > today
      ? None.OfType<PurchaseDate>()
      : Some.From(new PurchaseDate(date.Value, today));
  }
}
