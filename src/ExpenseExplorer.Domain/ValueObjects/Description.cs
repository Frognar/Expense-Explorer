namespace ExpenseExplorer.Domain.ValueObjects;

using System.Text.Json.Serialization;

public record Description
{
  [JsonConstructor]
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
