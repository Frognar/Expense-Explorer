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