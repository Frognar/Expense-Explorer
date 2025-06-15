using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.AddItem;

public sealed record AddReceiptItemRequest(
    Guid ReceiptId,
    string ItemName,
    string CategoryName,
    decimal Quantity,
    decimal UnitPrice,
    decimal? Discount,
    string? Description)
    : ICommand<AddReceiptItemResponse>;