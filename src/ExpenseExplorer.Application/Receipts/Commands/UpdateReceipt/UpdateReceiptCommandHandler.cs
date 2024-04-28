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
    return await (
      from patchModel in UpdateReceiptValidator.Validate(command)
      from id in Id.TryCreate(command.ReceiptId).ToResult(() => CommonFailures.InvalidReceiptId)
      from receipt in _receiptRepository.GetAsync(id, cancellationToken)
      from updated in Success.From(Update(receipt, patchModel))
      from version in _receiptRepository.SaveAsync(updated, cancellationToken)
      select updated.WithVersion(version).ClearChanges());
  }

  private static Receipt Update(Receipt receipt, ReceiptPatchModel patchModel)
  {
    return patchModel.PurchaseDate.Match(
      () => patchModel.Store.Match(() => receipt, receipt.CorrectStore),
      date => patchModel.Store.Match(() => receipt, receipt.CorrectStore).ChangePurchaseDate(date, patchModel.Today));
  }
}
