namespace ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

public sealed record ReceiptItemFilter(
    IEnumerable<string> Stores,
    IEnumerable<string> Items,
    IEnumerable<string> Categories,
    DateOnly? PurchaseDateFrom,
    DateOnly? PurchaseDateTo,
    decimal? UnitPriceMin,
    decimal? UnitPriceMax,
    decimal? QuantityMin,
    decimal? QuantityMax,
    decimal? DiscountMin,
    decimal? DiscountMax,
    decimal? TotalMin,
    decimal? TotalMax,
    string? Description)
{
    private static readonly ReceiptItemFilter Empty = new(
        [],
        [],
        [],
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null);

    public static ReceiptItemFilter StoresIn(IEnumerable<string> stores) =>
        Empty with { Stores = stores };

    public static ReceiptItemFilter ItemsIn(IEnumerable<string> items) =>
        Empty with { Items = items };

    public static ReceiptItemFilter CategoriesIn(IEnumerable<string> categories) =>
        Empty with { Categories = categories };

    public static ReceiptItemFilter PurchaseDateBetween(DateOnly? from, DateOnly? to) =>
        Empty with { PurchaseDateFrom = from, PurchaseDateTo = to };

    public static ReceiptItemFilter UnitPriceBetween(decimal? min, decimal? max) =>
        Empty with { UnitPriceMin = min, UnitPriceMax = max };

    public static ReceiptItemFilter QuantityBetween(decimal? min, decimal? max) =>
        Empty with { QuantityMin = min, QuantityMax = max };

    public static ReceiptItemFilter DiscountBetween(decimal? min, decimal? max) =>
        Empty with { DiscountMin = min, DiscountMax = max };

    public static ReceiptItemFilter TotalBetween(decimal? min, decimal? max) =>
        Empty with { TotalMin = min, TotalMax = max };

    public static ReceiptItemFilter DescriptionContains(string? description) =>
        Empty with { Description = description };
}