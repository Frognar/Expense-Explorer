namespace ExpenseExplorer.Domain.ValueObjects;

using System.Text.Json.Serialization;
using ExpenseExplorer.Domain.Exceptions;

public record Category
{
  [JsonConstructor]
  private Category(string name)
  {
    EmptyCategoryNameException.ThrowIfEmpty(name);
    Name = name.Trim();
  }

  public string Name { get; }

  public static Category Create(string name)
  {
    return new Category(name);
  }
}
