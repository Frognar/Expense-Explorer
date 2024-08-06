namespace ExpenseExplorer.Domain.ValueObjects;

using DotMaybe;

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
      : Some.With(new Store(name));
  }
}
