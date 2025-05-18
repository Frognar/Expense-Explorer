namespace ExpenseExplorer.Application.Receipts.DTO;

public sealed record ReceiptItem(
    string Item,
    string Category,
    decimal UnitPrice,
    decimal Quantity,
    decimal? Discount,
    string? Description);