using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct Description
{
  public static readonly Description Empty = new(string.Empty);

  private Description(string value)
  {
    Value = value.Trim();
  }

  public string Value { get; }

  public static Maybe<Description> TryCreate(string? value)
  {
    return Some.With(string.IsNullOrWhiteSpace(value) ? Empty : new Description(value));
  }
}
