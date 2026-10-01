namespace ExpenseExplorer.Api.Receipts.Import.Photo;

internal static partial class PhotoImportLog
{
    /// <summary>The lines as read help to see why a receipt was imported wrong.</summary>
    [LoggerMessage(Level = LogLevel.Information, Message = "Read {LineCount} lines from a receipt photo: {Lines}")]
    public static partial void LinesRead(this ILogger logger, int lineCount, IReadOnlyList<string> lines);

    [LoggerMessage(Level = LogLevel.Error, Message = "The OCR service could not read the photo")]
    public static partial void OcrFailed(this ILogger logger, Exception exception);
}
