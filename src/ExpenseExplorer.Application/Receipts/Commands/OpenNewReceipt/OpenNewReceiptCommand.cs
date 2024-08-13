using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed record OpenNewReceiptCommand(string StoreName, DateOnly PurchaseDate, DateOnly Today)
  : ICommand<Result<Receipt>>;
