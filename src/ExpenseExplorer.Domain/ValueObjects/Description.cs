namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct Description(string? Value)
{
  public string Value { get; } = Value?.Trim() ?? string.Empty;

  public static Description Create(string? value)
  {
    return new Description(value);
  }
}
