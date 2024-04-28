namespace ExpenseExplorer.Domain.ValueObjects;

using FunctionalCore.Monads;

public record Id
{
  private Id(string value)
  {
    ArgumentNullException.ThrowIfNull(value);
    Value = value.Trim();
  }

  public string Value { get; }

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
