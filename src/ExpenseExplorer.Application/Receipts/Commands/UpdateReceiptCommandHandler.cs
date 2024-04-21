namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public class UpdateReceiptCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<UpdateReceiptCommand, Either<Failure, Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Either<Failure, Receipt>> HandleAsync(
    UpdateReceiptCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Either<Failure, Receipt> eitherFailureOrReceipt = await Id.TryCreate(command.ReceiptId)
      .Match(
        () => Task.FromResult(Left.From<Failure, Receipt>(CommonFailures.InvalidReceiptId)),
        async receiptId => await Store.TryCreate(command.StoreName ?? string.Empty)
          .Match(
            async () => await _receiptRepository.GetAsync(receiptId, cancellationToken),
            async store
              => (await _receiptRepository.GetAsync(receiptId, cancellationToken))
              .MapRight(r => r.CorrectStore(store))));

    return await SaveAsync(eitherFailureOrReceipt, cancellationToken);
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
    return eitherFailureOrVersion.MapRight(v => receipt.WithVersion(v).ClearChanges());
  }
}
