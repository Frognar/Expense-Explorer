namespace ExpenseExplorer.Domain.ValueObjects;

using System.Text.Json.Serialization;
using ExpenseExplorer.Domain.Exceptions;

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
}
