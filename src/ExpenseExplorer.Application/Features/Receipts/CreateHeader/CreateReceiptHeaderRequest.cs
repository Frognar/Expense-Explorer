using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.CreateHeader;

public sealed record CreateReceiptHeaderRequest(string StoreName, DateOnly PurchaseDate, DateOnly Today)
    : ICommand<CreateReceiptHeaderResponse>;