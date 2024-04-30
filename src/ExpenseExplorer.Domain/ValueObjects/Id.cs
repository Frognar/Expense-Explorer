namespace ExpenseExplorer.Domain.ValueObjects;

using FunctionalCore.Monads;

public readonly record struct Id(string Value)
{
  public string Value { get; } = Value.Trim();

  public static Id Unique()
  {
    return new Id(Guid.NewGuid().ToString("N"));
  }

  public static Id Create(string value)
  {
    return new Id(value);
  }

  public static Maybe<Id> TryCreate(string value)
  {
    return string.IsNullOrWhiteSpace(value)
      ? None.OfType<Id>()
      : Some.From(new Id(value));
  }
}
