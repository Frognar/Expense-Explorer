namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;

public class OpenNewReceiptCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<OpenNewReceiptCommand, Either<Failure, Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Either<Failure, Receipt>> HandleAsync(
    OpenNewReceiptCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Validated<Receipt> validated = ReceiptValidator.Validate(command);
    Either<Failure, Receipt> either = validated.ToEither();
    return await either.FlatMapRight(r => Save(r, cancellationToken));
  }

  private async Task<Either<Failure, Receipt>> Save(Receipt receipt, CancellationToken cancellationToken)
  {
    var result = await _receiptRepository.Save(receipt, cancellationToken);
    return result.MapRight(_ => receipt.ClearChanges());
  }
}
