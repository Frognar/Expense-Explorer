namespace ExpenseExplorer.Domain;

public static class Id
{
  public static string Unique() => Guid.NewGuid().ToString("N");
}
