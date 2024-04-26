namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public class UpdateReceiptCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<UpdateReceiptCommand, Result<Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Receipt>> HandleAsync(
    UpdateReceiptCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Result<ReceiptPatchModel> resultOfPatchModel = UpdateReceiptValidator.Validate(command);
    Result<Receipt> resultOfReceipt = await UpdateReceiptAsync(
      resultOfPatchModel,
      command.ReceiptId,
      cancellationToken);

    return await SaveAsync(resultOfReceipt, cancellationToken);
  }

  private async Task<Result<Receipt>> UpdateReceiptAsync(
    Result<ReceiptPatchModel> resultOfPatchModel,
    string receiptId,
    CancellationToken cancellationToken)
  {
    return await Id.TryCreate(receiptId)
      .Match(
        () => Task.FromResult(Fail.OfType<Receipt>(CommonFailures.InvalidReceiptId)),
        async id => await UpdateReceiptAsync(resultOfPatchModel, id, cancellationToken));
  }

  private async Task<Result<Receipt>> UpdateReceiptAsync(
    Result<ReceiptPatchModel> resultOfPatchModel,
    Id receiptId,
    CancellationToken cancellationToken)
  {
    return await resultOfPatchModel.Match(
      failure => Task.FromResult(Fail.OfType<Receipt>(failure)),
      patchModel => UpdateReceiptAsync(patchModel, receiptId, cancellationToken));
  }

  private async Task<Result<Receipt>> UpdateReceiptAsync(
    ReceiptPatchModel patchModel,
    Id receiptId,
    CancellationToken cancellationToken)
  {
    Result<Receipt> resultOfReceipt = await _receiptRepository.GetAsync(receiptId, cancellationToken);
    return resultOfReceipt
      .Map(r => patchModel.Store.Match(() => r, r.CorrectStore))
      .Map(r => patchModel.PurchaseDate.Match(() => r, date => r.ChangePurchaseDate(date, patchModel.Today)));
  }

  private async Task<Result<Receipt>> SaveAsync(
    Result<Receipt> resultOfReceipt,
    CancellationToken cancellationToken)
  {
    return await resultOfReceipt.Match(
      failure => Task.FromResult(Fail.OfType<Receipt>(failure)),
      receipt => SaveAsync(receipt, cancellationToken));
  }

  private async Task<Result<Receipt>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    Result<Version> resultOfVersion = await _receiptRepository.SaveAsync(receipt, cancellationToken);
    return resultOfVersion.Map(v => receipt.WithVersion(v).ClearChanges());
  }
}
