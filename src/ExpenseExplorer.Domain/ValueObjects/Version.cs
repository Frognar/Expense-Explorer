namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct Version
{
  private Version(ulong value)
  {
    Value = value;
  }

  public ulong Value { get; }

  public static Version Create(ulong value)
  {
    return new Version(value);
  }

  public static Version New()
  {
    return new Version(ulong.MaxValue);
  }

  public Version Next()
  {
    return new Version(Value + 1);
  }
}
