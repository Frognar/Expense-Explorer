using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.Duplicate;

public sealed record DuplicateReceiptRequest(Guid ReceiptId, DateOnly Today)
    : ICommand<DuplicateReceiptResponse>;