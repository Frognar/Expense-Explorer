namespace ExpenseExplorer.WebApp.Models;

internal sealed record PurchaseDetails(
    Guid Id,
    string ItemName,
    string Category,
    decimal Quantity,
    decimal UnitPrice,
    decimal? Discount,
    string? Description)
{
    public decimal TotalPrice => Quantity * UnitPrice * (1 - (Discount ?? 0));
}