using System.Runtime.CompilerServices;
using ExpenseExplorer.Contracts.Logs;
using Microsoft.Extensions.Options;

namespace ExpenseExplorer.Api.Logs;

/// <summary>Reads the log files while Serilog keeps writing to today's one.</summary>
internal sealed class LogFiles(IOptions<LogOptions> options)
{
    private static readonly FileStreamOptions SharedRead = new()
    {
        Mode = FileMode.Open,
        Access = FileAccess.Read,
        Share = FileShare.ReadWrite | FileShare.Delete,
        Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
    };

    private string Folder => Path.GetFullPath(options.Value.Directory);

    public async Task<LogListResponse> ListAsync(LogQuery query, CancellationToken cancellationToken)
    {
        List<LogEntryResponse> matching = [];
        await foreach (string line in LinesOfAsync(query.Day, cancellationToken))
        {
            if (LogLine.Parse(line) is { } entry && query.Matches(entry))
            {
                matching.Add(entry);
            }
        }

        return new LogListResponse(query.Day, [.. FilesByDay().Select(file => file.Day).Distinct().OrderDescending()], query.PageOf(matching));
    }

    private async IAsyncEnumerable<string> LinesOfAsync(DateOnly day, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach ((string path, _) in FilesByDay().Where(file => file.Day == day).OrderBy(file => file.Path, StringComparer.Ordinal))
        {
            await using FileStream stream = new(path, SharedRead);
            using StreamReader reader = new(stream);
            while (await reader.ReadLineAsync(cancellationToken) is { } line)
            {
                yield return line;
            }
        }
    }

    private IEnumerable<(string Path, DateOnly Day)> FilesByDay() =>
        Directory.Exists(Folder)
            ? Directory.EnumerateFiles(Folder, LogFileName.SearchPattern)
                .Select(path => (Path: path, Day: LogFileName.DayOf(Path.GetFileName(path))))
                .Where(file => file.Day is not null)
                .Select(file => (file.Path, file.Day!.Value))
            : [];
}
