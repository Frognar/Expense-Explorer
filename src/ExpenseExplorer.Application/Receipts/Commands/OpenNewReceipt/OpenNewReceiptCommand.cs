namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Domain.Receipts;

public sealed record OpenNewReceiptCommand(string StoreName, DateOnly PurchaseDate, DateOnly Today)
  : ICommand<Result<Receipt>>;
