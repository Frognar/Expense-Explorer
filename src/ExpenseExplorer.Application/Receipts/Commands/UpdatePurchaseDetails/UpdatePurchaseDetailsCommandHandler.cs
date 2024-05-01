namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public sealed class UpdatePurchaseDetailsCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<UpdatePurchaseDetailsCommand, Result<Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Receipt>> HandleAsync(
    UpdatePurchaseDetailsCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    return await (
      from receiptId in Id.TryCreate(command.ReceiptId).ToResult(() => CommonFailures.InvalidReceiptId)
      from receipt in _receiptRepository.GetAsync(receiptId, cancellationToken)
      from purchaseId in Id.TryCreate(command.PurchaseId).ToResult(() => CommonFailures.InvalidPurchaseId)
      let purchase = receipt.Purchases.SingleOrDefault(p => p.Id == purchaseId)
      from result in purchase != default
        ? Success.From(receipt)
        : Fail.OfType<Receipt>(Failure.NotFound("Purchase not found", command.PurchaseId))
      select result);
  }
}
