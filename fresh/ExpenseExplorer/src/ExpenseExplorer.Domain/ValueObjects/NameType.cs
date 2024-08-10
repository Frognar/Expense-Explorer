using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct NameType(string Value);

public static class Name
{
  public static Maybe<NameType> Create(string? value)
    => string.IsNullOrWhiteSpace(value)
      ? None.OfType<NameType>()
      : Some.With(new NameType(value.Trim()));
}
