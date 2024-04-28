namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public record Item
{
  private Item(string name)
  {
    EmptyItemNameException.ThrowIfEmpty(name);
    Name = name.Trim();
  }

  public string Name { get; }

  public static Item Create(string name)
  {
    return new Item(name);
  }

  public static Maybe<Item> TryCreate(string name)
  {
    return string.IsNullOrWhiteSpace(name)
      ? None.OfType<Item>()
      : Some.From(new Item(name));
  }
}
