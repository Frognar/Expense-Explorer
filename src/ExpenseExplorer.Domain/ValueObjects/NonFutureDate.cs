namespace ExpenseExplorer.Domain.ValueObjects;

using FunctionalCore.Monads;

public readonly record struct NonFutureDate
{
  public static readonly NonFutureDate MinValue = new(DateOnly.MinValue);

  private NonFutureDate(DateOnly date)
  {
    Date = date;
  }

  public DateOnly Date { get; }

  public static Maybe<NonFutureDate> TryCreate(DateOnly date, DateOnly today)
  {
    return date <= today
      ? Some.From(new NonFutureDate(date))
      : None.OfType<NonFutureDate>();
  }
}
