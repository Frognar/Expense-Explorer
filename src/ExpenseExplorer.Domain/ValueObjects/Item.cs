namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;

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
}
