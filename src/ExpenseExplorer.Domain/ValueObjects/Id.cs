namespace ExpenseExplorer.Domain.ValueObjects;

public record Id
{
  public Id(string value)
  {
    ArgumentNullException.ThrowIfNull(value);
    Value = value.Trim();
  }

  public string Value { get; }

  public static Id Unique()
  {
    return new Id(Guid.NewGuid().ToString("N"));
  }
}
