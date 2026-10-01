namespace ExpenseExplorer.Api.Receipts.Import.Photo;

/// <summary>
/// The OCR finds the product name and its price as separate pieces of text. This puts back
/// together the lines as printed: pieces at the same height, read left to right.
/// </summary>
internal static class PrintedLines
{
    public static IReadOnlyList<string> Of(IReadOnlyList<OcrText> texts)
    {
        if (texts.Count == 0)
        {
            return [];
        }

        double slope = Median(texts.Where(IsWide).Select(text => text.Slope).DefaultIfEmpty(0));
        double sameLine = Median(texts.Select(text => text.Height)) / 2;

        // On a tilted photo a line runs at an angle; measuring height across that angle levels it.
        IEnumerable<(double Height, OcrText Text)> fromTop = texts
            .Select(text => (Height: text.Center.Y - (slope * text.Center.X), Text: text))
            .OrderBy(piece => piece.Height);

        List<List<(double Height, OcrText Text)>> lines = [];
        foreach ((double Height, OcrText Text) piece in fromTop)
        {
            if (lines.Count > 0 && piece.Height - lines[^1].Average(other => other.Height) < sameLine)
            {
                lines[^1].Add(piece);
            }
            else
            {
                lines.Add([piece]);
            }
        }

        return [.. lines.Select(line => string.Join(' ', line.OrderBy(piece => piece.Text.Center.X).Select(piece => piece.Text.Text.Trim())))];
    }

    /// <summary>Short pieces like "C" or "040" give a poor reading of the angle.</summary>
    private static bool IsWide(OcrText text) => text.Width > 2 * text.Height;

    private static double Median(IEnumerable<double> values)
    {
        double[] sorted = [.. values.Order()];
        return sorted.Length % 2 == 1
            ? sorted[sorted.Length / 2]
            : (sorted[(sorted.Length / 2) - 1] + sorted[sorted.Length / 2]) / 2;
    }
}
