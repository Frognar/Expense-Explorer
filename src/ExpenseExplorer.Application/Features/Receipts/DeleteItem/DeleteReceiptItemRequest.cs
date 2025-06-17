using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteItem;

public sealed record DeleteReceiptItemRequest(Guid ReceiptId, Guid ReceiptItemId)
    : ICommand<DeleteReceiptItemResponse>;