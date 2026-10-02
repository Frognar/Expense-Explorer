namespace ExpenseExplorer.Api.Logs;

/// <summary>The "Logs" section of appsettings: where the log panel looks for the files Serilog writes.</summary>
internal sealed class LogOptions
{
    public const string Section = "Logs";

    /// <summary>Must match the folder of the "File" sink in the "Serilog" section.</summary>
    public string Directory { get; set; } = "logs";
}
