using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.UpdateHeader;

public sealed record UpdateReceiptHeaderRequest(Guid ReceiptId, string StoreName, DateOnly PurchaseDate, DateOnly Today)
    : ICommand<UpdateReceiptHeaderResponse>;