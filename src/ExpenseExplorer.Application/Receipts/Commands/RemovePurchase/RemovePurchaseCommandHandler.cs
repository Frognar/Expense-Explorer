namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore;

public sealed class RemovePurchaseCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<RemovePurchaseCommand, Result<Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Receipt>> HandleAsync(
    RemovePurchaseCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    return await (
      from receiptId in Id.TryCreate(command.ReceiptId).ToResult(() => CommonFailures.InvalidReceiptId)
      from receipt in _receiptRepository.GetAsync(receiptId, cancellationToken)
      from purchaseId in Id.TryCreate(command.PurchaseId).ToResult(() => CommonFailures.InvalidPurchaseId)
      let receiptWithoutPurchase = receipt.RemovePurchase(purchaseId)
      from version in _receiptRepository.SaveAsync(receiptWithoutPurchase, cancellationToken)
      select receiptWithoutPurchase.WithVersion(version).ClearChanges());
  }
}
