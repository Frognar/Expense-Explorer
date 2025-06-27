using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.GetReceipt;

public sealed record GetReceiptByIdQuery(Guid ReceiptId) : IQuery<GetReceiptByIdResponse>;