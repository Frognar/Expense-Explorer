namespace ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

public sealed record ReceiptItemFilter(
    IEnumerable<string> Stores,
    IEnumerable<string> Items,
    IEnumerable<string> Categories,
    DateOnly? PurchaseDateFrom,
    DateOnly? PurchaseDateTo,
    decimal? TotalMin,
    decimal? TotalMax)
{
    public static ReceiptItemFilter StoresIn(IEnumerable<string> stores) =>
        new(stores, [], [], null, null, null, null);

    public static ReceiptItemFilter ItemsIn(IEnumerable<string> items) =>
        new([], items, [], null, null, null, null);

    public static ReceiptItemFilter CategoriesIn(IEnumerable<string> categories) =>
        new([], [], categories, null, null, null, null);

    public static ReceiptItemFilter PurchaseDateBetween(DateOnly? from, DateOnly? to) =>
        new([], [], [], from, to, null, null);

    public static ReceiptItemFilter TotalBetween(decimal? min, decimal? max) =>
        new([], [], [], null, null, min, max);
}