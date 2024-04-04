namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

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
    Either<Failure, Receipt> receipt = await SaveAsync(validated.ToEither(), cancellationToken);
    return receipt;
  }

  private async Task<Either<Failure, Receipt>> SaveAsync(
    Either<Failure, Receipt> either,
    CancellationToken cancellationToken)
  {
    return await either.Match(
      left => Task.FromResult(Left.From<Failure, Receipt>(left)),
      right => SaveAsync(right, cancellationToken));
  }

  private async Task<Either<Failure, Receipt>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    Either<Failure, Version> result = await _receiptRepository.SaveAsync(receipt, cancellationToken);
    return result.MapRight(_ => receipt.ClearChanges());
  }
}
