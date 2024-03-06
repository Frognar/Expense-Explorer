namespace ExpenseExplorer.Tests.Shared;

public static class Utils {
  public static readonly DateTime today = new(2000, 1, 1);
  public static readonly DateOnly todayDateOnly = new(today.Year, today.Month, today.Day);
}
