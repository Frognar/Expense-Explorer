namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;
using FunctionalCore.Validations;

public class AddPurchaseCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<AddPurchaseCommand, Either<Failure, Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Either<Failure, Receipt>> HandleAsync(
    AddPurchaseCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    var eitherFailureOrPurchase = PurchaseValidator.Validate(command).ToEither();
    var eitherFailureOrReceipt = await AddPurchaseAsync(eitherFailureOrPurchase, command.ReceiptId, cancellationToken);
    eitherFailureOrReceipt = await SaveAsync(eitherFailureOrReceipt, cancellationToken);
    return eitherFailureOrReceipt;
  }

  private async Task<Either<Failure, Receipt>> AddPurchaseAsync(
    Either<Failure, Purchase> eitherFailureOrPurchase,
    string receiptId,
    CancellationToken cancellationToken)
  {
    return await Id.TryCreate(receiptId)
      .Match(
        () => Task.FromResult(Left.From<Failure, Receipt>(CommonFailures.InvalidReceiptId)),
        async id => await AddPurchaseAsync(eitherFailureOrPurchase, id, cancellationToken));
  }

  private async Task<Either<Failure, Receipt>> AddPurchaseAsync(
    Either<Failure, Purchase> eitherFailureOrPurchase,
    Id receiptId,
    CancellationToken cancellationToken)
  {
    return await eitherFailureOrPurchase.Match(
      failure => Task.FromResult(Left.From<Failure, Receipt>(failure)),
      purchase => AddPurchaseAsync(purchase, receiptId, cancellationToken));
  }

  private async Task<Either<Failure, Receipt>> AddPurchaseAsync(
    Purchase purchase,
    Id receiptId,
    CancellationToken cancellationToken)
  {
    var eitherFailureOrReceipt = await _receiptRepository.GetAsync(receiptId, cancellationToken);
    return eitherFailureOrReceipt.MapRight(r => r.AddPurchase(purchase));
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
