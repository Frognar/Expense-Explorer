namespace ExpenseExplorer.WebApp.Models;

internal sealed class PurchaseDetails(
    Guid id,
    string itemName,
    string category,
    decimal quantity,
    decimal unitPrice,
    decimal? discount,
    string? description)
{
    public decimal TotalPrice => Quantity * UnitPrice * (1 - (Discount ?? 0));
    public Guid Id { get; init; } = id;
    public string ItemName { get; set; } = itemName;
    public string Category { get; set; } = category;
    public decimal Quantity { get; set; } = quantity;
    public decimal UnitPrice { get; set; } = unitPrice;
    public decimal? Discount { get; set; } = discount;
    public string? Description { get; set; } = description;
}

internal static class PurchaseDetailsExtensions
{
    public static PurchaseDetails MakeCopy(this PurchaseDetails purchase)
    {
        return new PurchaseDetails(purchase.Id,
            purchase.ItemName,
            purchase.Category,
            purchase.Quantity,
            purchase.UnitPrice,
            purchase.Discount,
            purchase.Description);
    }

    public static void Update(this PurchaseDetails purchase, PurchaseDetails newPurchase)
    {
        purchase.ItemName = newPurchase.ItemName;
        purchase.Category = newPurchase.Category;
        purchase.Quantity = newPurchase.Quantity;
        purchase.UnitPrice = newPurchase.UnitPrice;
        purchase.Discount = newPurchase.Discount;
        purchase.Description = newPurchase.Description;
    }
}