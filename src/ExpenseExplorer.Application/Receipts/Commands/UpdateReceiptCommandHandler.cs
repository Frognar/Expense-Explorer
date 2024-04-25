namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;
using FunctionalCore.Validations;

public class UpdateReceiptCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<UpdateReceiptCommand, Either<Failure, Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Either<Failure, Receipt>> HandleAsync(
    UpdateReceiptCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Either<Failure, UpdateReceiptPatchModel> eitherFailureOrPatchModel
      = UpdateReceiptValidator.Validate(command).ToEither();

    Either<Failure, Receipt> eitherFailureOrReceipt = await UpdateReceiptAsync(
      eitherFailureOrPatchModel,
      command.ReceiptId,
      cancellationToken);

    eitherFailureOrReceipt = await SaveAsync(eitherFailureOrReceipt, cancellationToken);
    return eitherFailureOrReceipt;
  }

  private async Task<Either<Failure, Receipt>> UpdateReceiptAsync(
    Either<Failure, UpdateReceiptPatchModel> eitherFailureOrPatchModel,
    string receiptId,
    CancellationToken cancellationToken)
  {
    return await Id.TryCreate(receiptId)
      .Match(
        () => Task.FromResult(Left.From<Failure, Receipt>(CommonFailures.InvalidReceiptId)),
        async id => await UpdateReceiptAsync(eitherFailureOrPatchModel, id, cancellationToken));
  }

  private async Task<Either<Failure, Receipt>> UpdateReceiptAsync(
    Either<Failure, UpdateReceiptPatchModel> eitherFailureOrPatchModel,
    Id receiptId,
    CancellationToken cancellationToken)
  {
    return await eitherFailureOrPatchModel.Match(
      failure => Task.FromResult(Left.From<Failure, Receipt>(failure)),
      patchModel => UpdateReceiptAsync(patchModel, receiptId, cancellationToken));
  }

  private async Task<Either<Failure, Receipt>> UpdateReceiptAsync(
    UpdateReceiptPatchModel patchModel,
    Id receiptId,
    CancellationToken cancellationToken)
  {
    Either<Failure, Receipt> eitherFailureOrReceipt = await _receiptRepository.GetAsync(receiptId, cancellationToken);
    return eitherFailureOrReceipt
      .MapRight(r => patchModel.Store.Match(() => r, r.CorrectStore))
      .MapRight(r => patchModel.PurchaseDate.Match(() => r, date => r.ChangePurchaseDate(date, patchModel.Today)));
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
