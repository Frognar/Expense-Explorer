using ExpenseExplorer.Contracts.Common;
using ExpenseExplorer.Contracts.Logs;

namespace ExpenseExplorer.Api.Logs;

/// <summary>A checked <see cref="LogListRequest"/>.</summary>
internal sealed record LogQuery(DateOnly Day, LogSeverity MinimumLevel, string? Search, int Page, int PageSize)
{
    public bool Matches(LogEntryResponse entry) =>
        entry.Level >= MinimumLevel && (Search is null || Searchable(entry).Any(Contains));

    /// <summary>The newest entries first; a page past the end is empty.</summary>
    public PageResponse<LogEntryResponse> PageOf(IReadOnlyCollection<LogEntryResponse> entries) =>
        new(
            [.. entries.OrderByDescending(entry => entry.Timestamp).Skip((Page - 1) * PageSize).Take(PageSize)],
            Page,
            PageSize,
            entries.Count);

    private bool Contains(string? text) => text?.Contains(Search!, StringComparison.OrdinalIgnoreCase) == true;

    private static IEnumerable<string?> Searchable(LogEntryResponse entry) =>
        [entry.Message, entry.Source, entry.Exception, .. entry.Properties.Values];
}
