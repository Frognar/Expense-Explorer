namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public class OpenNewReceiptCommandHandler
{
  public OpenNewReceiptCommandHandler(IReceiptRepository repository)
  {
  }

#pragma warning disable CA1822
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
  public async Task<Either<Failure, Receipt>> HandleAsync(OpenNewReceiptCommand command)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning restore CA1822
  {
    ArgumentNullException.ThrowIfNull(command);
    var receipt = Receipt.New(
      Store.Create(command.StoreName),
      PurchaseDate.Create(command.PurchaseDate, command.Today));

    return Right.From<Failure, Receipt>(receipt);
  }
}
