using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.UpdateItem;

public record UpdateReceiptItemRequest(
    Guid ReceiptItemId,
    Guid ReceiptId,
    string ItemName,
    string CategoryName,
    decimal Quantity,
    decimal UnitPrice,
    decimal? Discount,
    string? Description)
    : ICommand<UpdateReceiptItemResponse>;