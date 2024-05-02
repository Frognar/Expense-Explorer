namespace ExpenseExplorer.Domain.ValueObjects;

using FunctionalCore.Monads;

public readonly record struct Description(string? Value)
{
  public string Value { get; } = Value?.Trim() ?? string.Empty;

  public static Description Create(string? value)
  {
    return new Description(value);
  }

  public static Maybe<Description> TryCreate(string? value)
  {
    return Some.From(new Description(value));
  }
}
