namespace ExpenseExplorer.Application.Features.Receipts.GetReceipts;

public sealed record ReceiptFilter(
    IEnumerable<string> Stores,
    DateOnly? PurchaseDateFrom,
    DateOnly? PurchaseDateTo,
    decimal? TotalMin,
    decimal? TotalMax)
{
    public static ReceiptFilter StoresIn(IEnumerable<string> stores) =>
        new(stores, null, null, null, null);

    public static ReceiptFilter PurchaseDateBetween(DateOnly? from, DateOnly? to) =>
        new([], from, to, null, null);

    public static ReceiptFilter TotalBetween(decimal? min, decimal? max) =>
        new([], null, null, min, max);
}