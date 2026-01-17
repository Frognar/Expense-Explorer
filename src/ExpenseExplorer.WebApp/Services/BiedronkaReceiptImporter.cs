using System.Globalization;
using System.Text.Json;
using DotResult;
using ExpenseExplorer.Application;
using ExpenseExplorer.WebApp.Models;

namespace ExpenseExplorer.WebApp.Services;

internal sealed record BiedronkaReceiptImport(
    DateOnly PurchaseDate,
    IReadOnlyList<PurchaseDetails> Purchases);

internal static class BiedronkaReceiptImporter
{
    private const string DefaultCategory = "Spożywcze";
    private static readonly CultureInfo PolishCulture = new("pl-PL");

    public static Result<BiedronkaReceiptImport> Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Failure.Validation(
                code: "IMPORT_EMPTY_FILE",
                message: "Import file is empty",
                metadata: new Dictionary<string, object>());
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;

            if (!TryGetPurchaseDate(root, out DateOnly purchaseDate))
            {
                return Failure.Validation(
                    code: "IMPORT_MISSING_HEADER_DATE",
                    message: "Missing header date",
                    metadata: new Dictionary<string, object>());
            }

            if (!root.TryGetProperty("body", out JsonElement body) || body.ValueKind != JsonValueKind.Array)
            {
                return Failure.Validation(
                    code: "IMPORT_MISSING_BODY",
                    message: "Missing receipt body",
                    metadata: new Dictionary<string, object>());
            }

            List<PurchaseDetails> purchases = [];
            PurchaseDetails? lastItem = null;
            decimal voucherTotal = 0m;

            foreach (JsonElement line in body.EnumerateArray())
            {
                if (line.TryGetProperty("sellLine", out JsonElement sellLine))
                {
                    if (sellLine.TryGetProperty("isStorno", out JsonElement isStornoElement) &&
                        isStornoElement.ValueKind == JsonValueKind.True)
                    {
                        return Failure.Validation(
                            code: "IMPORT_STORNO_NOT_SUPPORTED",
                            message: "Storno lines are not supported",
                            metadata: new Dictionary<string, object>());
                    }

                    string rawName = sellLine.GetProperty("name").GetString() ?? "";
                    string vatId = sellLine.TryGetProperty("vatId", out JsonElement vatElement)
                        ? vatElement.GetString() ?? ""
                        : "";

                    string name = NormalizeItemName(rawName, vatId);
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return Failure.Validation(
                            code: "IMPORT_EMPTY_ITEM_NAME",
                            message: "Item name is empty",
                            metadata: new Dictionary<string, object>());
                    }

                    string quantityRaw = sellLine.GetProperty("quantity").GetString() ?? "";
                    if (!TryParseDecimal(quantityRaw, out decimal quantity) || quantity <= 0)
                    {
                        return Failure.Validation(
                            code: "IMPORT_INVALID_QUANTITY",
                            message: "Invalid item quantity",
                            metadata: new Dictionary<string, object>
                            {
                                { "Quantity", quantityRaw }
                            });
                    }

                    if (!TryParseMoneyFromGrosz(sellLine.GetProperty("price"), out decimal unitPrice) || unitPrice <= 0)
                    {
                        return Failure.Validation(
                            code: "IMPORT_INVALID_UNIT_PRICE",
                            message: "Invalid unit price",
                            metadata: new Dictionary<string, object>());
                    }

                    PurchaseDetails purchase = new(Guid.Empty, name, DefaultCategory, quantity, unitPrice, null, null);
                    purchases.Add(purchase);
                    lastItem = purchase;
                    continue;
                }

                if (line.TryGetProperty("discountLine", out JsonElement discountLine))
                {
                    if (lastItem is null)
                    {
                        return Failure.Validation(
                            code: "IMPORT_DISCOUNT_WITHOUT_ITEM",
                            message: "Discount line without a previous item",
                            metadata: new Dictionary<string, object>());
                    }

                    if (discountLine.TryGetProperty("isStorno", out JsonElement isStornoElement) &&
                        isStornoElement.ValueKind == JsonValueKind.True)
                    {
                        return Failure.Validation(
                            code: "IMPORT_STORNO_NOT_SUPPORTED",
                            message: "Storno lines are not supported",
                            metadata: new Dictionary<string, object>());
                    }

                    bool isDiscount = !discountLine.TryGetProperty("isDiscount", out JsonElement isDiscountElement) ||
                                      isDiscountElement.ValueKind == JsonValueKind.True;

                    if (!isDiscount)
                    {
                        return Failure.Validation(
                            code: "IMPORT_SURCHARGE_NOT_SUPPORTED",
                            message: "Surcharge lines are not supported",
                            metadata: new Dictionary<string, object>());
                    }

                    bool isPercent = discountLine.TryGetProperty("isPercent", out JsonElement isPercentElement) &&
                                     isPercentElement.ValueKind == JsonValueKind.True;

                    decimal discountAmount;
                    if (isPercent)
                    {
                        decimal baseAmount = TryParseMoneyFromGrosz(discountLine.GetProperty("base"), out decimal baseMoney)
                            ? baseMoney
                            : 0m;

                        decimal percent = discountLine.GetProperty("value").GetDecimal();
                        discountAmount = baseAmount * (percent / 100m);
                    }
                    else
                    {
                        if (!TryParseMoneyFromGrosz(discountLine.GetProperty("value"), out discountAmount))
                        {
                            return Failure.Validation(
                                code: "IMPORT_INVALID_DISCOUNT",
                                message: "Invalid discount value",
                                metadata: new Dictionary<string, object>());
                        }
                    }

                    if (discountAmount <= 0)
                    {
                        continue;
                    }

                    lastItem.Discount = (lastItem.Discount ?? 0m) + discountAmount;
                    continue;
                }

                if (line.TryGetProperty("discountVat", out JsonElement discountVat))
                {
                    string name = discountVat.TryGetProperty("name", out JsonElement nameElement)
                        ? nameElement.GetString() ?? ""
                        : "";

                    bool isDiscount = !discountVat.TryGetProperty("isDiscount", out JsonElement isDiscountElement) ||
                                      isDiscountElement.ValueKind == JsonValueKind.True;

                    if (isDiscount
                        && name.Equals("Voucher", StringComparison.OrdinalIgnoreCase)
                        && TryParseMoneyFromGrosz(discountVat.GetProperty("value"), out decimal voucherValue))
                    {
                        voucherTotal += voucherValue;
                    }
                }
            }

            if (purchases.Count == 0)
            {
                return Failure.Validation(
                    code: "IMPORT_NO_ITEMS",
                    message: "No sell lines found",
                    metadata: new Dictionary<string, object>());
            }

            if (voucherTotal > 0)
            {
                Result<Unit> voucherResult = ApplyVoucherDiscounts(purchases, voucherTotal);
                if (voucherResult.IsFailure())
                {
                    return Failure.Validation(
                        code: "IMPORT_VOUCHER_DISTRIBUTION_FAILED",
                        message: "Failed to distribute voucher discount",
                        metadata: new Dictionary<string, object>());
                }
            }

            foreach (PurchaseDetails purchase in purchases)
            {
                if (purchase.Discount is not null && purchase.Discount.Value > purchase.PriceBeforeDiscount)
                {
                    return Failure.Validation(
                        code: "IMPORT_DISCOUNT_EXCEEDS_TOTAL",
                        message: "Discount exceeds item total",
                        metadata: new Dictionary<string, object>());
                }
            }

            return Success.From(new BiedronkaReceiptImport(purchaseDate, purchases));
        }
        catch (JsonException ex)
        {
            return Failure.Validation(
                code: "IMPORT_INVALID_JSON",
                message: ex.Message,
                metadata: new Dictionary<string, object>
                {
                    { "Error", ex.Message }
                });
        }
    }

    private static bool TryGetPurchaseDate(JsonElement root, out DateOnly purchaseDate)
    {
        purchaseDate = default;

        if (!root.TryGetProperty("header", out JsonElement header) || header.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        foreach (JsonElement headerItem in header.EnumerateArray())
        {
            if (!headerItem.TryGetProperty("headerData", out JsonElement headerData))
            {
                continue;
            }

            if (!headerData.TryGetProperty("date", out JsonElement dateElement) ||
                dateElement.ValueKind != JsonValueKind.String)
            {
                continue;
            }

            if (DateTimeOffset.TryParse(
                    dateElement.GetString(),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out DateTimeOffset parsed))
            {
                purchaseDate = DateOnly.FromDateTime(parsed.UtcDateTime);
                return true;
            }
        }

        return false;
    }

    private static bool TryParseDecimal(string value, out decimal result)
    {
        return decimal.TryParse(value.Trim(), NumberStyles.Number, PolishCulture, out result) ||
               decimal.TryParse(value.Trim().Replace(",", ".", StringComparison.Ordinal),
                   NumberStyles.Number,
                   CultureInfo.InvariantCulture,
                   out result);
    }

    private static bool TryParseMoneyFromGrosz(JsonElement element, out decimal result)
    {
        result = 0m;
        if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out decimal numeric))
        {
            result = numeric / 100m;
            return true;
        }

        if (element.ValueKind == JsonValueKind.String &&
            decimal.TryParse(element.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out numeric))
        {
            result = numeric / 100m;
            return true;
        }

        return false;
    }

    private static string NormalizeItemName(string name, string vatId)
    {
        string trimmed = string.Join(" ", name.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        if (!string.IsNullOrWhiteSpace(vatId) &&
            trimmed.EndsWith(vatId, StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed[..^vatId.Length].TrimEnd();
        }

        return trimmed;
    }

    private static Result<Unit> ApplyVoucherDiscounts(List<PurchaseDetails> purchases, decimal voucherTotal)
    {
        decimal subtotal = purchases.Sum(p => p.PriceAfterDiscount);
        if (subtotal <= 0 || voucherTotal <= 0)
        {
            return Unit.Instance;
        }

        decimal roundedVoucherTotal = Math.Round(voucherTotal, 2, MidpointRounding.AwayFromZero);
        List<decimal> shares = new(purchases.Count);

        foreach (PurchaseDetails purchase in purchases)
        {
            decimal share = Math.Round(
                roundedVoucherTotal * (purchase.PriceAfterDiscount / subtotal),
                2,
                MidpointRounding.AwayFromZero);
            shares.Add(share);
        }

        decimal diff = Math.Round(roundedVoucherTotal - shares.Sum(), 2, MidpointRounding.AwayFromZero);
        if (diff != 0m)
        {
            if (diff > 0m)
            {
                decimal remaining = diff;
                for (int i = 0; i < purchases.Count && remaining > 0m; i++)
                {
                    decimal capacity = purchases[i].PriceAfterDiscount - shares[i];
                    while (remaining > 0m && capacity >= 0.01m)
                    {
                        shares[i] += 0.01m;
                        remaining -= 0.01m;
                        capacity -= 0.01m;
                    }
                }

                if (remaining > 0m)
                {
                    return Failure.Validation(
                        code: "IMPORT_VOUCHER_DISTRIBUTION_FAILED",
                        message: "Not enough capacity to distribute voucher",
                        metadata: new Dictionary<string, object>());
                }
            }
            else
            {
                decimal remaining = Math.Abs(diff);
                for (int i = 0; i < purchases.Count && remaining > 0m; i++)
                {
                    while (remaining > 0m && shares[i] >= 0.01m)
                    {
                        shares[i] -= 0.01m;
                        remaining -= 0.01m;
                    }
                }

                if (remaining > 0m)
                {
                    return Failure.Validation(
                        code: "IMPORT_VOUCHER_DISTRIBUTION_FAILED",
                        message: "Unable to reduce voucher distribution",
                        metadata: new Dictionary<string, object>());
                }
            }
        }

        for (int i = 0; i < purchases.Count; i++)
        {
            if (shares[i] <= 0m)
            {
                continue;
            }

            purchases[i].Discount = (purchases[i].Discount ?? 0m) + shares[i];
        }

        return Unit.Instance;
    }

    private static bool IsFailure(this Result<Unit> result)
    {
        bool failed = false;
        result.Match(
            failure: _ =>
            {
                failed = true;
                return Unit.Instance;
            },
            success: _ => Unit.Instance);

        return failed;
    }
}
