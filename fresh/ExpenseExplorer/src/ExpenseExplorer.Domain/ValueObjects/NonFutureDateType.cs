using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct NonFutureDateType(DateOnly Value);

public static class NonFutureDate
{
  public static Maybe<NonFutureDateType> Create(DateOnly value, DateOnly today)
    => value > today
      ? None.OfType<NonFutureDateType>()
      : Some.With(new NonFutureDateType(value));
}
