using System.Globalization;
using System.Text.Json;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Api.Receipts.Import;

/// <summary>
/// Reads the JSON e-receipt from the Biedronka app. Prices in the file are in grosze. Discounts
/// belong to the line above them; a voucher is spread over all lines in proportion to their value.
/// A bottle deposit becomes a line of its own, since the receipt does not say which product it is for;
/// a returned deposit lowers the whole receipt like a voucher.
/// </summary>
internal static class BiedronkaReceiptParser
{
    public const string Store = "Biedronka";
    public const string DefaultCategory = "Spożywcze";

    private static readonly CultureInfo Polish = new("pl-PL");

    public static Result<ImportReceipt> Parse(string json, DateOnly today, TimeZoneInfo timeZone)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Fail<ImportReceipt>("Import.EmptyFile", "The file is empty.");
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(json);
        }
        catch (JsonException)
        {
            return Fail<ImportReceipt>("Import.InvalidJson", "The file is not valid JSON.");
        }

        using (document)
        {
            JsonElement root = document.RootElement;
            return ResultCombine.Combine(
                StoreName.Create(Store),
                PurchaseDateOf(root, timeZone).Bind(date => PurchaseDate.Create(date, today).ForTarget("file")),
                LinesOf(root).Bind(ToPurchases),
                (store, date, purchases) => new ImportReceipt(store, date, purchases));
        }
    }

    /// <summary>A line while the file is being read; discounts add up before it becomes a <see cref="Purchase"/>.</summary>
    private sealed record Line(string Name, decimal Quantity, decimal Amount, decimal Discount)
    {
        public decimal Total => Amount - Discount;
    }

    private static Result<DateOnly> PurchaseDateOf(JsonElement root, TimeZoneInfo timeZone)
    {
        IEnumerable<string> dates = Array(root, "header")
            .Select(header => Property(header, "headerData") is { } data ? Text(data, "date") : null)
            .OfType<string>();

        foreach (string date in dates)
        {
            if (DateTimeOffset.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset moment))
            {
                return Result.Success(DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(moment, timeZone).DateTime));
            }
        }

        return Fail<DateOnly>("Import.MissingDate", "The file does not say when the purchase was made.");
    }

    private static Result<IReadOnlyList<Line>> LinesOf(JsonElement root)
    {
        if (Property(root, "body") is not { ValueKind: JsonValueKind.Array } body)
        {
            return Fail<IReadOnlyList<Line>>("Import.MissingBody", "The file has no receipt lines.");
        }

        List<Line> lines = [];
        decimal voucher = 0m;
        foreach (JsonElement entry in body.EnumerateArray())
        {
            Result<bool> read = entry switch
            {
                _ when Property(entry, "sellLine") is { } sell => ItemLine(sell).Map(line =>
                {
                    lines.Add(line);
                    return true;
                }),
                _ when Property(entry, "discountLine") is { } discount => DiscountLine(discount).Bind(value =>
                {
                    if (lines.Count == 0)
                    {
                        return Fail<bool>("Import.DiscountWithoutItem", "A discount comes before any item.");
                    }

                    lines[^1] = lines[^1] with { Discount = lines[^1].Discount + value };
                    return Result.Success(true);
                }),
                _ when Property(entry, "discountVat") is { } vat && IsVoucher(vat) => Grosze(vat, "value").Map(value =>
                {
                    voucher += value;
                    return true;
                }),
                _ when Property(entry, "pack") is { } pack && IsTrue(pack, "isNegative") => Grosze(pack, "total").Map(value =>
                {
                    voucher += value;
                    return true;
                }),
                _ when Property(entry, "pack") is { } pack => ItemLine(pack).Map(line =>
                {
                    lines.Add(line);
                    return true;
                }),
                _ => Result.Success(false),
            };

            if (!read.IsSuccess)
            {
                return Result.Failure<IReadOnlyList<Line>>(read.Errors);
            }
        }

        return lines.Count == 0
            ? Fail<IReadOnlyList<Line>>("Import.NoItems", "The file has no items.")
            : WithVoucher(lines, voucher);
    }

    /// <summary>A product or a deposit. The line total printed on the receipt wins over price times quantity.</summary>
    private static Result<Line> ItemLine(JsonElement line)
    {
        if (IsTrue(line, "isStorno"))
        {
            return Fail<Line>("Import.Storno", "Cancelled (storno) lines are not supported.");
        }

        string name = CleanName(Text(line, "name") ?? "", Text(line, "vatId") ?? "");
        return ResultCombine.Combine(
                ParseQuantity(Text(line, "quantity")),
                Grosze(line, "price"),
                (quantity, price) => (quantity, price))
            .Bind(parts => parts.price <= 0m
                ? Fail<Line>("Import.InvalidPrice", $"Item '{name}' has no price.")
                : LineAmount(line, parts.price, parts.quantity).Map(amount => new Line(name, parts.quantity, amount, 0m)));
    }

    private static Result<decimal> LineAmount(JsonElement line, decimal price, decimal quantity) =>
        Property(line, "total") is null
            ? Result.Success(decimal.Round(price * quantity, Money.MaxDecimalPlaces, MidpointRounding.AwayFromZero))
            : Grosze(line, "total");

    private static Result<decimal> DiscountLine(JsonElement line)
    {
        if (IsTrue(line, "isStorno"))
        {
            return Fail<decimal>("Import.Storno", "Cancelled (storno) lines are not supported.");
        }

        if (Property(line, "isDiscount") is { ValueKind: JsonValueKind.False })
        {
            return Fail<decimal>("Import.Surcharge", "Surcharges are not supported.");
        }

        if (!IsTrue(line, "isPercent"))
        {
            return Grosze(line, "value").Map(Positive);
        }

        return Grosze(line, "base").Bind(baseAmount =>
            Property(line, "value") is { ValueKind: JsonValueKind.Number } percent && percent.TryGetDecimal(out decimal rate)
                ? Result.Success(Positive(decimal.Round(baseAmount * rate / 100m, Money.MaxDecimalPlaces, MidpointRounding.AwayFromZero)))
                : Fail<decimal>("Import.InvalidDiscount", "A percentage discount has no rate."));
    }

    private static Result<IReadOnlyList<Line>> WithVoucher(List<Line> lines, decimal voucher)
    {
        if (voucher == 0m)
        {
            return Result.Success<IReadOnlyList<Line>>(lines);
        }

        decimal[] totals = [.. lines.Select(line => line.Total)];
        if (voucher > totals.Sum())
        {
            return Fail<IReadOnlyList<Line>>("Import.VoucherTooLarge", "The voucher is larger than the receipt.");
        }

        IReadOnlyList<decimal> shares = Proportional.Split(voucher, totals);
        return Result.Success<IReadOnlyList<Line>>(
            [.. lines.Select((line, index) => line with { Discount = line.Discount + shares[index] })]);
    }

    private static Result<IReadOnlyList<Purchase>> ToPurchases(IReadOnlyList<Line> lines) =>
        ResultCombine.Sequence(lines.Select(ToPurchase)).ForTarget("file");

    private static Result<Purchase> ToPurchase(Line line) =>
        ResultCombine.Combine(
            ItemName.Create(line.Name),
            CategoryName.Create(DefaultCategory),
            Domain.Receipts.Quantity.Create(line.Quantity)
                .Bind(quantity => ResultCombine.Combine(
                    Money.Create(line.Amount),
                    Money.Create(line.Discount),
                    (amount, discount) => (amount, discount))
                    .Bind(money => LinePrice.Create(quantity, money.amount, money.discount))),
            (item, category, price) => new Purchase(item, category, price, null));

    /// <summary>Collapses spaces and drops the VAT group letter the receipt prints after the name.</summary>
    private static string CleanName(string name, string vatId)
    {
        string collapsed = string.Join(' ', name.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return vatId.Length > 0 && collapsed.EndsWith($" {vatId}", StringComparison.OrdinalIgnoreCase)
            ? collapsed[..^(vatId.Length + 1)]
            : collapsed;
    }

    private static Result<decimal> ParseQuantity(string? text) =>
        text is not null
        && (decimal.TryParse(text.Trim(), NumberStyles.Number, Polish, out decimal quantity)
            || decimal.TryParse(text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out quantity))
        && quantity > 0m
            ? Result.Success(quantity)
            : Fail<decimal>("Import.InvalidQuantity", $"'{text}' is not a valid quantity.");

    /// <summary>An amount the file gives in grosze, as a number or a string.</summary>
    private static Result<decimal> Grosze(JsonElement element, string name) =>
        Property(element, name) switch
        {
            { ValueKind: JsonValueKind.Number } number when number.TryGetDecimal(out decimal grosze) => Result.Success(grosze / 100m),
            { ValueKind: JsonValueKind.String } text when decimal.TryParse(text.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal grosze) => Result.Success(grosze / 100m),
            _ => Fail<decimal>("Import.InvalidAmount", $"'{name}' is not a valid amount."),
        };

    private static bool IsVoucher(JsonElement vat) =>
        Property(vat, "isDiscount") is not { ValueKind: JsonValueKind.False }
        && string.Equals(Text(vat, "name"), "Voucher", StringComparison.OrdinalIgnoreCase);

    private static decimal Positive(decimal value) => Math.Max(value, 0m);

    private static JsonElement? Property(JsonElement element, string name) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out JsonElement value) ? value : null;

    private static string? Text(JsonElement element, string name) =>
        Property(element, name) is { ValueKind: JsonValueKind.String } text ? text.GetString() : null;

    private static bool IsTrue(JsonElement element, string name) =>
        Property(element, name) is { ValueKind: JsonValueKind.True };

    private static JsonElement[] Array(JsonElement element, string name) =>
        Property(element, name) is { ValueKind: JsonValueKind.Array } array ? [.. array.EnumerateArray()] : [];

    private static Result<T> Fail<T>(string code, string message) =>
        Result.Failure<T>(new Error(code, message, Target: "file"));
}
