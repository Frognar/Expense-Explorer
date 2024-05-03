namespace ExpenseExplorer.Domain.ValueObjects;

using FunctionalCore.Monads;

public readonly record struct PurchaseDate
{
  public static readonly PurchaseDate MinValue = new(DateOnly.MinValue);

  private PurchaseDate(DateOnly date)
  {
    Date = date;
  }

  public DateOnly Date { get; }

  public static Maybe<PurchaseDate> TryCreate(DateOnly date, DateOnly today)
  {
    return date <= today
      ? Some.From(new PurchaseDate(date))
      : None.OfType<PurchaseDate>();
  }
}
