namespace ExpenseExplorer.Application.Receipts;

public sealed record ReceiptFilter(
    IEnumerable<string> Stores,
    DateOnly? PurchaseDateFrom,
    DateOnly? PurchaseDateTo,
    decimal? TotalCostMin,
    decimal? TotalCostMax)
{
    public static ReceiptFilter StoresIn(IEnumerable<string> stores) =>
        new(stores, null, null, null, null);

    public static ReceiptFilter PurchaseDateBetween(DateOnly? from, DateOnly? to) =>
        new([], from, to, null, null);

    public static ReceiptFilter TotalCostBetween(decimal? min, decimal? max) =>
        new([], null, null, min, max);
}