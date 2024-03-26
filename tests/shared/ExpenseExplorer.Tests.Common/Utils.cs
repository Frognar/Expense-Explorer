namespace ExpenseExplorer.Tests.Common;

public static class Utils
{
  public static DateTime Today { get; } = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

  public static DateOnly TodayDateOnly { get; } = new(Today.Year, Today.Month, Today.Day);
}
