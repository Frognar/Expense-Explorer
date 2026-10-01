namespace ExpenseExplorer.Api.Receipts.Import.Photo;

/// <summary>A piece of text the OCR found on the photo, with the corners of its box in pixels.</summary>
internal sealed record OcrText(string Text, OcrPoint TopLeft, OcrPoint TopRight, OcrPoint BottomRight, OcrPoint BottomLeft)
{
    public OcrPoint Center => new(
        (TopLeft.X + TopRight.X + BottomRight.X + BottomLeft.X) / 4,
        (TopLeft.Y + TopRight.Y + BottomRight.Y + BottomLeft.Y) / 4);

    public double Width => TopRight.X - TopLeft.X;

    public double Height => (BottomLeft.Y - TopLeft.Y + (BottomRight.Y - TopRight.Y)) / 2;

    /// <summary>How much the top edge rises or falls per pixel to the right; non-zero when the photo is tilted.</summary>
    public double Slope => Width > 0 ? (TopRight.Y - TopLeft.Y) / Width : 0;
}

internal readonly record struct OcrPoint(double X, double Y);
