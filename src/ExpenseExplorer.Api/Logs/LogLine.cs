using System.Text.Json;
using ExpenseExplorer.Contracts.Logs;

namespace ExpenseExplorer.Api.Logs;

/// <summary>
/// Reads one line of a compact JSON (CLEF) log file: <c>@t</c> time, <c>@m</c> rendered message
/// (or <c>@mt</c> template), <c>@l</c> level (left out for Information), <c>@x</c> exception;
/// other <c>@</c> fields are Serilog's own, the rest are properties of the event.
/// </summary>
internal static class LogLine
{
    private const string SourceProperty = "SourceContext";

    /// <summary>Null for a line that is not a log entry, e.g. one Serilog is still writing.</summary>
    public static LogEntryResponse? Parse(string line)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(line);
            return Entry(document.RootElement);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static LogEntryResponse? Entry(JsonElement json)
    {
        if (json.ValueKind != JsonValueKind.Object
            || Text(json, "@t") is not { } time
            || !DateTimeOffset.TryParse(time, System.Globalization.CultureInfo.InvariantCulture, out DateTimeOffset timestamp))
        {
            return null;
        }

        Dictionary<string, string> properties = json.EnumerateObject()
            .Where(property => IsEventProperty(property.Name))
            .ToDictionary(property => Unescape(property.Name), property => Text(property.Value), StringComparer.Ordinal);
        properties.Remove(SourceProperty, out string? source);

        return new LogEntryResponse(
            timestamp,
            Level(Text(json, "@l")),
            Text(json, "@m") ?? Text(json, "@mt") ?? "",
            source,
            Text(json, "@x"),
            properties);
    }

    private static LogSeverity Level(string? name) =>
        Enum.TryParse(name, ignoreCase: false, out LogSeverity level) && Enum.IsDefined(level) ? level : LogSeverity.Information;

    // "@@" escapes a property whose own name starts with "@".
    private static bool IsEventProperty(string name) => !name.StartsWith('@') || name.StartsWith("@@", StringComparison.Ordinal);

    private static string Unescape(string name) => name.StartsWith("@@", StringComparison.Ordinal) ? name[1..] : name;

    private static string? Text(JsonElement json, string name) =>
        json.TryGetProperty(name, out JsonElement value) ? Text(value) : null;

    private static string Text(JsonElement value) =>
        value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : value.GetRawText();
}
