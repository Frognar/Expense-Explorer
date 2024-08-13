using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct Item
{
  private Item(string name)
  {
    Name = name.Trim();
  }

  public string Name { get; }

  public static Maybe<Item> TryCreate(string name)
  {
    return string.IsNullOrWhiteSpace(name)
      ? None.OfType<Item>()
      : Some.With(new Item(name));
  }
}
