namespace ExpenseExplorer.Domain.ValueObjects;

using System.Text.Json.Serialization;

public record Id
{
  [JsonConstructor]
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
}
