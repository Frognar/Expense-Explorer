namespace ExpenseExplorer.Domain.ValueObjects;

public record Description
{
  private Description(string? value)
  {
    Value = value?.Trim() ?? string.Empty;
  }

  public string Value { get; }

  public static Description Create(string? value)
  {
    return new Description(value);
  }
}
