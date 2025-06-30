namespace ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

public sealed record ReceiptItemOrder(string OrderBy, bool Descending)
{
    public static ReceiptItemOrder Id => new("Id", false);
    public static ReceiptItemOrder Store => new("Store", false);
    public static ReceiptItemOrder PurchaseDate => new("PurchaseDate", false);
    public static ReceiptItemOrder Item => new("Item", false);
    public static ReceiptItemOrder Category => new("Category", false);
    public static ReceiptItemOrder UnitPrice => new("UnitPrice", false);
    public static ReceiptItemOrder Quantity => new("Quantity", false);
    public static ReceiptItemOrder Discount => new("Discount", false);
    public static ReceiptItemOrder Total => new("Total", false);
    public static ReceiptItemOrder Description => new("Description", false);
}