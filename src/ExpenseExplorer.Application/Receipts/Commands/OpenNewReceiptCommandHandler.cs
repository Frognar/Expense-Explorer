namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;

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
    Validated<Receipt> receipt = ReceiptValidator.Validate(command);
    return receipt.ToEither().MapLeft(f => (Failure)f);
  }
}
