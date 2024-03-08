namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;

public record PurchaseDate
{
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
}
