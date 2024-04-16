namespace ExpenseExplorer.Domain.ValueObjects;

using System.Text.Json.Serialization;
using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public record PurchaseDate
{
  public static readonly PurchaseDate MinValue = new(DateOnly.MinValue, DateOnly.MinValue);

  [JsonConstructor]
  private PurchaseDate(DateOnly date)
  {
    Date = date;
  }

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

  public static Maybe<PurchaseDate> TryCreate(DateOnly date, DateOnly today)
  {
    return date > today
      ? None.OfType<PurchaseDate>()
      : Some.From(new PurchaseDate(date, today));
  }
}
