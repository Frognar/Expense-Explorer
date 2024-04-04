namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

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
    return await eitherFailureOrPurchase.Match(
      failure => Task.FromResult(Either<Failure, Receipt>.Left(failure)),
      purchase => AddPurchaseAsync(purchase, Id.Create(receiptId), cancellationToken));
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
    return eitherFailureOrVersion.MapRight(_ => receipt.ClearChanges());
  }
}
