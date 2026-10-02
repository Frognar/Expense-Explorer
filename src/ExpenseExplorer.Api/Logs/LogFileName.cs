using System.Globalization;
using System.Text.RegularExpressions;

namespace ExpenseExplorer.Api.Logs;

/// <summary>
/// Names of the daily files Serilog writes in compact JSON, e.g. <c>expense-explorer-20261002.clef</c>,
/// or <c>expense-explorer-20261002_001.clef</c> when it had to start a second file that day.
/// </summary>
internal static partial class LogFileName
{
    public const string SearchPattern = "*.clef";

    public static DateOnly? DayOf(string fileName) =>
        DailyFile().Match(fileName) is { Success: true } match
        && DateOnly.TryParseExact(match.Groups["day"].Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly day)
            ? day
            : null;

    [GeneratedRegex(@"-(?<day>\d{8})(_\d+)?\.clef$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 1000)]
    private static partial Regex DailyFile();
}
