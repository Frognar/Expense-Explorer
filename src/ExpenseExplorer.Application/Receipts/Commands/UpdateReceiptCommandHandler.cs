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
    Either<Failure, Receipt> eitherFailureOrReceipt = await GetReceiptAsync(command.ReceiptId, cancellationToken);
    eitherFailureOrReceipt = eitherFailureOrReceipt
      .MapRight(CorrectStoreIfNotEmpty(command.StoreName))
      .MapRight(ChangePurchaseDateIfNotEmpty(command.PurchaseDate, command.Today));

    eitherFailureOrReceipt = await SaveAsync(eitherFailureOrReceipt, cancellationToken);
    return eitherFailureOrReceipt;
  }

  private static Func<Receipt, Receipt> CorrectStoreIfNotEmpty(string? storeName)
  {
    return receipt => CorrectStoreIfNotEmpty(receipt, storeName ?? string.Empty);
  }

  private static Receipt CorrectStoreIfNotEmpty(Receipt receipt, string storeName)
  {
    return Store.TryCreate(storeName).Match(() => receipt, receipt.CorrectStore);
  }

  private static Func<Receipt, Receipt> ChangePurchaseDateIfNotEmpty(DateOnly? purchaseDate, DateOnly today)
  {
    return receipt => ChangePurchaseDateIfNotEmpty(receipt, purchaseDate, today);
  }

  private static Receipt ChangePurchaseDateIfNotEmpty(Receipt receipt, DateOnly? purchaseDate, DateOnly today)
  {
    return PurchaseDate.TryCreate(purchaseDate, today).Match(() => receipt, d => receipt.ChangePurchaseDate(d, today));
  }

  private async Task<Either<Failure, Receipt>> GetReceiptAsync(string receiptId, CancellationToken cancellationToken)
  {
    return await Id.TryCreate(receiptId)
      .Match(
        () => Task.FromResult(Left.From<Failure, Receipt>(CommonFailures.InvalidReceiptId)),
        id => _receiptRepository.GetAsync(id, cancellationToken));
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
    Either<Failure, Version> eitherFailureOrVersion = await _receiptRepository.SaveAsync(receipt, cancellationToken);
    return eitherFailureOrVersion.MapRight(v => receipt.WithVersion(v).ClearChanges());
  }
}
