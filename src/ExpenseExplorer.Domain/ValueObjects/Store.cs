namespace ExpenseExplorer.Domain.ValueObjects;

using System.Text.Json.Serialization;
using ExpenseExplorer.Domain.Exceptions;

public record Store
{
  [JsonConstructor]
  private Store(string name)
  {
    EmptyStoreNameException.ThrowIfEmpty(name);
    Name = name.Trim();
  }

  public string Name { get; }

  public static Store Create(string name)
  {
    return new Store(name);
  }
}
