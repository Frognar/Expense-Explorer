namespace ExpenseExplorer.Domain.ValueObjects;

using FunctionalCore.Monads;

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
      : Some.From(new Item(name));
  }
}
