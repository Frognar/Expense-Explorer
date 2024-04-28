namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

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

  public static Maybe<Store> TryCreate(string name)
  {
    return string.IsNullOrWhiteSpace(name)
      ? None.OfType<Store>()
      : Some.From(new Store(name));
  }
}
