using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed record UpdateReceiptCommand(string ReceiptId, string? StoreName, DateOnly? PurchaseDate, DateOnly Today)
  : ICommand<Result<Receipt>>;
