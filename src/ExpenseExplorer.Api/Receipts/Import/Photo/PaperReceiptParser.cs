using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Api.Receipts.Import.Photo;

/// <summary>
/// What was read from a photo of a paper receipt, with what the user should check: the total
/// printed on the receipt (to compare with the sum of the lines), whether the date was found,
/// and the lines that could not be read.
/// </summary>
internal sealed record PaperReceipt(
    ImportReceipt Receipt,
    decimal? PrintedTotal,
    bool PurchaseDateFound,
    IReadOnlyList<string> SkippedLines);

/// <summary>
/// Reads the printed lines of a Polish fiscal receipt ("paragon fiskalny"). The law makes item
/// lines look alike in every shop: name, quantity "x" unit price, amount and the VAT letter, as in
/// <c>SER KOZI 160G C   1 x10,65 10,65C</c>. A discount line follows the item it lowers
/// (<c>OPUST KABANOSY C   -2,50C</c>). OCR makes mistakes, so a line that cannot be read is
/// skipped and reported rather than failing the whole receipt.
/// </summary>
internal static partial class PaperReceiptParser
{
    public const string DefaultCategory = "Spożywcze";
    public const string UnknownStore = "Nieznany sklep";

    /// <summary>Chains whose company name on the receipt differs from the name people use, or is printed in capitals.</summary>
    private static readonly (string Printed, string Store)[] KnownStores =
    [
        ("JERONIMO MARTINS", "Biedronka"),
        ("BIEDRONKA", "Biedronka"),
        ("DINO", "Dino"),
        ("LIDL", "Lidl"),
        ("KAUFLAND", "Kaufland"),
        ("ZABKA", "Żabka"),
        ("AUCHAN", "Auchan"),
        ("CARREFOUR", "Carrefour"),
        ("NETTO", "Netto"),
        ("ALDI", "Aldi"),
        ("STOKROTKA", "Stokrotka"),
        ("LEWIATAN", "Lewiatan"),
        ("POLOMARKET", "POLOmarket"),
        ("INTERMARCHE", "Intermarché"),
        ("ROSSMANN", "Rossmann"),
        ("HEBE", "Hebe"),
        ("PEPCO", "Pepco"),
    ];

    public static Result<PaperReceipt> Parse(IReadOnlyList<string> lines, DateOnly today)
    {
        string[] normalized = [.. lines.Select(Normalize)];
        int bodyStart = BodyStart(lines, normalized);
        int bodyEnd = Array.FindIndex(normalized, bodyStart, IsAfterItems) is var end and >= 0 ? end : lines.Count;

        Body body = ReadBody(lines.Take(bodyEnd).Skip(bodyStart));
        if (body.Lines.Count == 0)
        {
            return Fail<PaperReceipt>("Import.NothingRead", "No receipt lines could be read from the photo.");
        }

        DateOnly? printedDate = PrintedDate(lines, today);
        return ResultCombine.Combine(
            StoreName.Create(StoreOf(lines, normalized[..bodyStart])),
            PurchaseDate.Create(printedDate ?? today, today),
            ResultCombine.Sequence(body.Lines.Select(ToPurchase)),
            (store, date, purchases) => new PaperReceipt(
                new ImportReceipt(store, date, purchases),
                PrintedTotal(normalized.Skip(bodyEnd)),
                printedDate is not null,
                body.Skipped))
            .ForTarget("file");
    }

    /// <summary>A line while the receipt is being read; a discount below it is added before it becomes a <see cref="Purchase"/>.</summary>
    private sealed record Line(string Name, decimal Quantity, decimal Amount, decimal Discount);

    private sealed record Body(IReadOnlyList<Line> Lines, IReadOnlyList<string> Skipped);

    private static Body ReadBody(IEnumerable<string> printed)
    {
        List<Line> lines = [];
        List<string> skipped = [];

        // A long name sometimes wraps, leaving the quantity and amount alone on the next line.
        string? nameAbove = null;
        foreach (string text in printed.Select(text => text.Trim()).Where(text => text.Length > 0))
        {
            if (ItemLine().Match(text) is { Success: true } item)
            {
                string name = CleanName(item.Groups["name"].Value);
                if (name.Length == 0 && nameAbove is not null)
                {
                    name = nameAbove;
                }
                else if (nameAbove is not null)
                {
                    skipped.Add(nameAbove);
                }

                nameAbove = null;
                if (name.Length == 0)
                {
                    skipped.Add(text);
                    continue;
                }

                decimal quantity = Number(item.Groups["quantity"].Value);
                decimal price = Number(item.Groups["price"].Value);
                decimal amount = item.Groups["amount"].Success
                    ? Number(item.Groups["amount"].Value)
                    : decimal.Round(quantity * price, Money.MaxDecimalPlaces, MidpointRounding.AwayFromZero);
                lines.Add(new Line(name, quantity, amount, 0m));
                continue;
            }

            if (nameAbove is not null)
            {
                skipped.Add(nameAbove);
                nameAbove = null;
            }

            if (DiscountLine().Match(text) is { Success: true } discount
                && lines.Count > 0
                && lines[^1].Discount + Number(discount.Groups["value"].Value) <= lines[^1].Amount)
            {
                lines[^1] = lines[^1] with { Discount = lines[^1].Discount + Number(discount.Groups["value"].Value) };
            }
            else if (!AnyAmount().IsMatch(text))
            {
                nameAbove = text;
            }
            else
            {
                skipped.Add(text);
            }
        }

        if (nameAbove is not null)
        {
            skipped.Add(nameAbove);
        }

        return new Body(lines, skipped);
    }

    /// <summary>
    /// Items follow the "PARAGON FISKALNY" title. Its large letters are sometimes not read at all;
    /// then everything above the first item line is the header.
    /// </summary>
    private static int BodyStart(IReadOnlyList<string> lines, string[] normalized)
    {
        int title = Array.FindIndex(normalized, line => line.Contains("PARAGON FISKALNY", StringComparison.Ordinal));
        if (title >= 0)
        {
            return title + 1;
        }

        int firstItem = lines.ToList().FindIndex(line => ItemLine().IsMatch(line.Trim()));
        return Math.Max(firstItem, 0);
    }

    /// <summary>
    /// The block of totals and VAT starts here; no items follow it. The sum of all discounts
    /// ("OPUSTY ŁĄCZNIE") opens it on some receipts and must not be taken for a discount on the last item.
    /// </summary>
    private static bool IsAfterItems(string line) =>
        line.StartsWith("SPRZEDAZ", StringComparison.Ordinal)
        || line.StartsWith("SUMA", StringComparison.Ordinal)
        || line.StartsWith("PTU", StringComparison.Ordinal)
        || line.StartsWith("OPUSTY", StringComparison.Ordinal)
        || line.StartsWith("RABATY", StringComparison.Ordinal)
        || line.Contains("LACZNIE", StringComparison.Ordinal);

    private static decimal? PrintedTotal(IEnumerable<string> normalizedTail) =>
        normalizedTail
            .Select(line => TotalLine().Match(line))
            .Where(match => match.Success)
            .Select(match => (decimal?)Number(match.Groups["value"].Value))
            .FirstOrDefault();

    /// <summary>The first date on the receipt that is not in the future; shops print it at the top or the bottom.</summary>
    private static DateOnly? PrintedDate(IReadOnlyList<string> lines, DateOnly today) =>
        lines
            .SelectMany(line => DateText().Matches(line))
            .Select(DateOf)
            .FirstOrDefault(date => date is not null && date <= today);

    private static DateOnly? DateOf(Match match)
    {
        (string year, string month, string day) = match.Groups["year"].Success
            ? (match.Groups["year"].Value, match.Groups["month"].Value, match.Groups["day"].Value)
            : (match.Groups["year2"].Value, match.Groups["month2"].Value, match.Groups["day2"].Value);
        return DateOnly.TryParseExact($"{year}-{month}-{day}", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date)
            ? date
            : null;
    }

    /// <summary>
    /// A known chain named in the header, otherwise its first line (the company name) without
    /// the legal form.
    /// </summary>
    private static string StoreOf(IReadOnlyList<string> lines, string[] header)
    {
        string? known = KnownStores
            .Where(store => header.Any(line => Regex.IsMatch(line, $@"\b{store.Printed}\b", RegexOptions.None, TimeSpan.FromSeconds(1))))
            .Select(store => store.Store)
            .FirstOrDefault();
        if (known is not null)
        {
            return known;
        }

        string company = header.Length > 0 ? LegalForm().Replace(lines[0], "").Trim(' ', ',', '.') : "";
        return company.Length > 0 ? company[..Math.Min(company.Length, StoreName.MaxLength)] : UnknownStore;
    }

    private static Result<Purchase> ToPurchase(Line line) =>
        ResultCombine.Combine(
            ItemName.Create(line.Name[..Math.Min(line.Name.Length, ItemName.MaxLength)]),
            CategoryName.Create(DefaultCategory),
            Domain.Receipts.Quantity.Create(line.Quantity)
                .Bind(quantity => ResultCombine.Combine(
                    Money.Create(line.Amount),
                    Money.Create(line.Discount),
                    (amount, discount) => (amount, discount))
                    .Bind(money => LinePrice.Create(quantity, money.amount, money.discount))),
            (item, category, price) => new Purchase(item, category, price, null));

    /// <summary>Collapses spaces and drops the VAT letter (A–G) printed after the name.</summary>
    private static string CleanName(string name)
    {
        string collapsed = string.Join(' ', name.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return VatLetter().Replace(collapsed, "");
    }

    /// <summary>Capitals without Polish letters and with single spaces, since OCR reads diacritics unreliably.</summary>
    private static string Normalize(string line)
    {
        string upper = line.ToUpperInvariant().Replace('Ł', 'L');
        StringBuilder plain = new(upper.Length);
        foreach (char letter in upper.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
            {
                plain.Append(letter);
            }
        }

        return string.Join(' ', plain.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    /// <summary>Receipts use a decimal comma; OCR sometimes reads it as a dot, or as both (",.").</summary>
    private static decimal Number(string text) =>
        decimal.Parse(Separator().Replace(text, "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

    private static Result<T> Fail<T>(string code, string message) =>
        Result.Failure<T>(new Error(code, message, Target: "file"));

    // "SER ZANETTI 100G C 1 x7,49 7,49C". The amount may be glued to the price ("1 x10,9910,99C") or
    // missing; the VAT letter at the end is often misread, so any single character is accepted there.
    [GeneratedRegex(@"^(?:(?<name>.*?)\s+)?(?<quantity>\d+(?:[.,]+\d{1,3})?)\s*[x×*]\s*(?<price>\d+[.,]+\d{2})(?:\s*(?<amount>\d+[.,]+\d{2}))?\s*\S?$", RegexOptions.IgnoreCase, 1000)]
    private static partial Regex ItemLine();

    [GeneratedRegex(@"^.*?-\s*(?<value>\d+[.,]+\d{2})\s*\S?$", RegexOptions.None, 1000)]
    private static partial Regex DiscountLine();

    [GeneratedRegex(@"\d+[.,]+\d{2}", RegexOptions.None, 1000)]
    private static partial Regex AnyAmount();

    [GeneratedRegex(@"^SUMA(?!\s*PTU)\D*(?<value>\d+[.,]+\d{2})", RegexOptions.None, 1000)]
    private static partial Regex TotalLine();

    [GeneratedRegex(@"(?<year>20\d{2})[-./](?<month>\d{2})[-./](?<day>\d{2})|(?<day2>\d{2})[-./](?<month2>\d{2})[-./](?<year2>20\d{2})", RegexOptions.None, 1000)]
    private static partial Regex DateText();

    [GeneratedRegex("[.,]+", RegexOptions.None, 1000)]
    private static partial Regex Separator();

    [GeneratedRegex(@"\s+[A-G]$", RegexOptions.None, 1000)]
    private static partial Regex VatLetter();

    [GeneratedRegex(@"\b(?:S\.?\s?A\.?|SP\.?\s?Z\s?O\.?\s?O\.?|SP\.?\s?J\.?|SP\.?\s?K\.?)(?=\s|,|$)", RegexOptions.IgnoreCase, 1000)]
    private static partial Regex LegalForm();
}
