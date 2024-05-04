namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Domain.Receipts;
using FunctionalCore.Monads;

public sealed record UpdateReceiptCommand(string ReceiptId, string? StoreName, DateOnly? PurchaseDate, DateOnly Today)
  : ICommand<Result<Receipt>>;
