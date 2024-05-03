namespace ExpenseExplorer.Domain.ValueObjects;

using FunctionalCore.Monads;

public readonly record struct Store
{
  private Store(string name)
  {
    Name = name.Trim();
  }

  public string Name { get; }

  public static Maybe<Store> TryCreate(string name)
  {
    return string.IsNullOrWhiteSpace(name)
      ? None.OfType<Store>()
      : Some.From(new Store(name));
  }
}
