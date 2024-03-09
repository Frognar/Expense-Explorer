namespace ExpenseExplorer.Tests.Common;

public static class Utils
{
  public static readonly DateTime Today = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
  public static readonly DateOnly TodayDateOnly = new(Today.Year, Today.Month, Today.Day);
}
