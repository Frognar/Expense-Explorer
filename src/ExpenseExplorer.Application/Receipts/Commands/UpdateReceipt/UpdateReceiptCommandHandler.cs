using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed class UpdateReceiptCommandHandler(IReceiptRepository receiptRepository)
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
      let updated = Update(receipt, patchModel)
      from version in _receiptRepository.SaveAsync(updated, cancellationToken)
      select updated.WithVersion(version).ClearChanges());
  }

  private static Receipt Update(Receipt receipt, ReceiptPatchModel patchModel)
  {
    return receipt
      .Apply(r => patchModel.PurchaseDate.Match(() => r, pd => r.ChangePurchaseDate(pd, patchModel.Today)))
      .Apply(r => patchModel.Store.Match(() => r, r.CorrectStore));
  }
}
