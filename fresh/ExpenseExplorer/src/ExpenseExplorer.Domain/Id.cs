using DotMaybe;

namespace ExpenseExplorer.Domain;

public static class Id
{
  public static string Unique() => Guid.NewGuid().ToString("N");

  public static Maybe<string> Create(string value)
    => Guid.TryParse(value, out _)
      ? value
      : None.OfType<string>();
}
