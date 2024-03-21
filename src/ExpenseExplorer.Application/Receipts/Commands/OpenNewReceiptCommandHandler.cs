namespace ExpenseExplorer.Application.Receipts.Commands;

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
    Validated<Receipt> validated = ReceiptValidator.Validate(command);
    Either<Failure, Receipt> either = validated.ToEither();
    return await either.FlatMapRight(Save);
  }

  private async Task<Either<Failure, Receipt>> Save(Receipt receipt)
  {
    var result = await repository.Save(receipt);
    return result.MapRight(_ => receipt.ClearChanges());
  }
}
