namespace ExpenseExplorer.Application.Receipts;

public sealed record ReceiptOrder(string OrderBy, bool Descending)
{
    public static ReceiptOrder Id => new("Id", false);
    public static ReceiptOrder Store => new("Store", false);
    public static ReceiptOrder PurchaseDate => new("PurchaseDate", false);
    public static ReceiptOrder TotalCost => new("TotalCost", false);
}