using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.Duplicate;

public record DuplicateReceiptRequest(Guid ReceiptId, DateOnly Today)
    : ICommand<DuplicateReceiptResponse>;