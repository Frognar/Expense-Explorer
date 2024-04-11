namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Domain.Receipts;
using FunctionalCore.Monads;

public record OpenNewReceiptCommand(string StoreName, DateOnly PurchaseDate, DateOnly Today)
  : ICommand<Either<Failure, Receipt>>;
