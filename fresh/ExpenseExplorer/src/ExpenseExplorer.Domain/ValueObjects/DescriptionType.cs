namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct DescriptionType(string Value);

public static class Description
{
  public static DescriptionType Create(string? value) => new(value ?? string.Empty);
}
