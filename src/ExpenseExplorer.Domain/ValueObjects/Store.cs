namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;

public record Store
{
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
