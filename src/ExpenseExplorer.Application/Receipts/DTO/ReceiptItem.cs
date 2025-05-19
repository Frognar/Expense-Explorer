using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.DTO;

public sealed record ReceiptItem(
    Item Item,
    Category Category,
    Money UnitPrice,
    Quantity Quantity,
    Money? Discount,
    Description? Description);