namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Application.Commands;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Domain.Receipts;

public record OpenNewReceiptCommand(string StoreName, DateOnly PurchaseDate, DateOnly Today)
  : ICommand<Either<Failure, Receipt>>;
