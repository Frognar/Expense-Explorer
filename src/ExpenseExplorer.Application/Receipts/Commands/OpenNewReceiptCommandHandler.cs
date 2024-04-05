namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Facts;

public class OpenNewReceiptCommandHandler(IReceiptRepository receiptRepository, IFactBus factBus)
  : ICommandHandler<OpenNewReceiptCommand, Either<Failure, Receipt>>
{
  private readonly IFactBus _factBus = factBus;
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Either<Failure, Receipt>> HandleAsync(
    OpenNewReceiptCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    var eitherFailureOrReceipt = ReceiptValidator.Validate(command).ToEither();
    eitherFailureOrReceipt = await SaveAsync(eitherFailureOrReceipt, cancellationToken);
    return eitherFailureOrReceipt;
  }

  private async Task<Either<Failure, Receipt>> SaveAsync(
    Either<Failure, Receipt> eitherFailureOrReceipt,
    CancellationToken cancellationToken)
  {
    return await eitherFailureOrReceipt.Match(
      failure => Task.FromResult(Left.From<Failure, Receipt>(failure)),
      receipt => SaveAsync(receipt, cancellationToken));
  }

  private async Task<Either<Failure, Receipt>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    var eitherFailureOrVersion = await _receiptRepository.SaveAsync(receipt, cancellationToken);
    await Task.WhenAll(receipt.UnsavedChanges.Select(PublishTask(cancellationToken)));
    return eitherFailureOrVersion.MapRight(_ => receipt.ClearChanges());
  }

  private Func<Fact, Task> PublishTask(CancellationToken cancellationToken)
    => fact => _factBus.PublishAsync(fact, cancellationToken);
}
