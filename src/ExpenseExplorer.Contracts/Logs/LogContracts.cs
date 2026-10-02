using ExpenseExplorer.Contracts.Common;

namespace ExpenseExplorer.Contracts.Logs;

/// <summary>How serious a log entry is, from the least to the most serious; names match Serilog's levels.</summary>
public enum LogSeverity
{
    Verbose,
    Debug,
    Information,
    Warning,
    Error,
    Fatal,
}

/// <summary>
/// Query string of <c>GET /api/v1/logs</c>. Without a day the API shows today's entries;
/// without a level it shows every level. Entries come newest first.
/// </summary>
public sealed record LogListRequest
{
    public const int DefaultPageSize = 50;

    public const int MaxPageSize = 200;

    public DateOnly? Day { get; init; }

    /// <summary>Entries of this level and every more serious one.</summary>
    public LogSeverity? Level { get; init; }

    /// <summary>Text to look for in the message, source, exception and properties, ignoring case.</summary>
    public string? Search { get; init; }

    public int? Page { get; init; }

    public int? PageSize { get; init; }
}

/// <summary>One logged event. <see cref="Properties"/> holds every other property as text, JSON for structured values.</summary>
public sealed record LogEntryResponse(
    DateTimeOffset Timestamp,
    LogSeverity Level,
    string Message,
    string? Source,
    string? Exception,
    IReadOnlyDictionary<string, string> Properties);

/// <summary>Entries of one day, and every day that has a log file, newest first.</summary>
public sealed record LogListResponse(
    DateOnly Day,
    IReadOnlyList<DateOnly> Days,
    PageResponse<LogEntryResponse> Entries);
