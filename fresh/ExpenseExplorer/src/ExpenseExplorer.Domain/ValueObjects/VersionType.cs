namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct VersionType(ulong Value);

public static class Version
{
  public static VersionType Create(ulong value) => new(value);

  public static VersionType New() => new(ulong.MaxValue);
}
