namespace ExpenseExplorer.WebApp.Models;

internal sealed record ReceiptItemDetails(
    Guid Id,
    Guid ReceiptId,
    string Store,
    DateOnly PurchaseDate,
    string Item,
    string Category,
    decimal UnitPrice,
    decimal Quantity,
    decimal? Discount,
    decimal Total,
    string? Description);