namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Domain.Receipts;
using FunctionalCore.Monads;

public record UpdateReceiptCommand(string ReceiptId, string? StoreName, DateOnly? PurchaseDate, DateOnly Today)
  : ICommand<Result<Receipt>>;