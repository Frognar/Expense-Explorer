namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct Version(ulong Value)
{
  public static Version Create(ulong value)
  {
    return new Version(value);
  }

  public static Version New()
  {
    return new Version(ulong.MaxValue);
  }
}
