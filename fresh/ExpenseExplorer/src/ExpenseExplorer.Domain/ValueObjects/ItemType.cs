using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct ItemType(string Value);

public static class Item
{
  public static Maybe<ItemType> Create(string? value)
    => string.IsNullOrWhiteSpace(value)
      ? None.OfType<ItemType>()
      : Some.With(new ItemType(value.Trim()));
}
