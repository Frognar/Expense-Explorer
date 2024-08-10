using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct StoreType(string Value);

public static class Store
{
  public static Maybe<StoreType> Create(string? value)
    => string.IsNullOrWhiteSpace(value)
      ? None.OfType<StoreType>()
      : Some.With(new StoreType(value.Trim()));
}
