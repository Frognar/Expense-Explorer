namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public readonly record struct PurchaseDate(DateOnly Date, DateOnly Today)
{
  public static readonly PurchaseDate MinValue = new(DateOnly.MinValue, DateOnly.MinValue);

  private readonly DateOnly _date = GetOrThrow(Date, Today);

  public DateOnly Date
  {
    get => _date;
    init => _date = GetOrThrow(value, Today);
  }

  private DateOnly Today { get; } = Today;

  public static PurchaseDate Create(DateOnly date, DateOnly today)
  {
    return new PurchaseDate(date, today);
  }

  public static Maybe<PurchaseDate> TryCreate(DateOnly date, DateOnly today)
  {
    return IsValid(date, today)
      ? Some.From(new PurchaseDate(date, today))
      : None.OfType<PurchaseDate>();
  }

  private static DateOnly GetOrThrow(DateOnly date, DateOnly today)
  {
    if (IsValid(date, today))
    {
      return date;
    }

    throw new FutureDateException(date, today);
  }

  private static bool IsValid(DateOnly date, DateOnly today)
  {
    return date <= today;
  }
}
