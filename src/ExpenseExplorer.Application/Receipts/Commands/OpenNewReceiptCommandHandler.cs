namespace ExpenseExplorer.Application.Receipts.Commands;

using System.Diagnostics;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;

public class OpenNewReceiptCommandHandler(IReceiptRepository repository)
{
  private readonly IReceiptRepository repository = repository;

  public async Task<Either<Failure, Receipt>> HandleAsync(OpenNewReceiptCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Validated<Receipt> receipt = ReceiptValidator.Validate(command);
    if (receipt.IsValid)
    {
      Receipt r = receipt.Match(_ => throw new UnreachableException(), r => r);
      await repository.Save(r);
    }

    return receipt.ToEither().MapLeft(f => (Failure)f);
  }
}
