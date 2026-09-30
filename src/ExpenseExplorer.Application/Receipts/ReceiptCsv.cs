using System.Globalization;
using System.Text;
using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Application.Receipts;

/// <summary>
/// A receipt as CSV: the receipt on top, then one row per line. Numbers use a dot as the decimal
/// separator; text is quoted when needed and never starts a spreadsheet formula.
/// </summary>
public static class ReceiptCsv
{
    private static readonly char[] CharactersNeedingQuotes = [',', '"', '\r', '\n'];
    private static readonly char[] FormulaStarts = ['=', '+', '-', '@', '\t', '\r'];

    public static string Write(ReceiptResponse receipt)
    {
        ArgumentNullException.ThrowIfNull(receipt);

        StringBuilder csv = new();
        AppendRow(csv, "Store", "PurchaseDate", "Total");
        AppendRow(csv, Text(receipt.Store), receipt.PurchaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), Amount(receipt.Total));
        csv.Append("\r\n");
        AppendRow(csv, "Item", "Category", "Quantity", "UnitPrice", "Amount", "Discount", "Total", "Description");
        foreach (ReceiptItemResponse item in receipt.Items)
        {
            AppendRow(
                csv,
                Text(item.Item),
                Text(item.Category),
                Number(item.Quantity),
                Number(item.UnitPrice),
                Amount(item.Amount),
                Amount(item.Discount),
                Amount(item.Total),
                Text(item.Description ?? ""));
        }

        return csv.ToString();
    }

    /// <summary>A file name made of the store and the date, safe on every system.</summary>
    public static string FileName(ReceiptResponse receipt)
    {
        ArgumentNullException.ThrowIfNull(receipt);

        string store = new([.. receipt.Store.Select(c => char.IsLetterOrDigit(c) ? c : '-')]);
        return $"{store}-{receipt.PurchaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}.csv";
    }

    private static void AppendRow(StringBuilder csv, params string[] fields) =>
        csv.Append(string.Join(',', fields)).Append("\r\n");

    private static string Number(decimal value) => value.ToString("0.####", CultureInfo.InvariantCulture);

    private static string Amount(decimal value) => value.ToString("0.00", CultureInfo.InvariantCulture);

    private static string Text(string value)
    {
        string safe = value.Length > 0 && FormulaStarts.Contains(value[0]) ? $"'{value}" : value;
        return safe.IndexOfAny(CharactersNeedingQuotes) >= 0
            ? $"\"{safe.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : safe;
    }
}
