namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public class AddPurchaseCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<AddPurchaseCommand, Result<Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Receipt>> HandleAsync(
    AddPurchaseCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    return await (
      from purchase in AddPurchaseValidator.Validate(command)
      from id in Id.TryCreate(command.ReceiptId).ToResult(() => CommonFailures.InvalidReceiptId)
      from receipt in _receiptRepository.GetAsync(id, cancellationToken)
      let receiptWithPurchase = receipt.AddPurchase(purchase)
      from version in _receiptRepository.SaveAsync(receiptWithPurchase, cancellationToken)
      select receiptWithPurchase.WithVersion(version).ClearChanges());
  }
}
