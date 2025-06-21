using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteHeader;

public sealed record DeleteReceiptHeaderRequest(Guid ReceiptId) : ICommand<DeleteReceiptHeaderResponse>;