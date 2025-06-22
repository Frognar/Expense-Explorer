using DotMaybe;

namespace ExpenseExplorer.Application.Features.Receipts.GetReceipt;

public sealed record GetReceiptByIdResponse(Maybe<ReceiptDetails> Receipt);