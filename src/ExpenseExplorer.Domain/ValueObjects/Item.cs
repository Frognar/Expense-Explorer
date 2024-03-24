namespace ExpenseExplorer.Domain.ValueObjects;

using System.Text.Json.Serialization;
using ExpenseExplorer.Domain.Exceptions;

public record Item
{
  [JsonConstructor]
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
