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
      from patchModel in UpdatePurchaseDetailsValidator.Validate(command)
      from receiptId in Id.TryCreate(command.ReceiptId).ToResult(() => CommonFailures.InvalidReceiptId)
      from receipt in _receiptRepository.GetAsync(receiptId, cancellationToken)
      from purchaseId in Id.TryCreate(command.PurchaseId).ToResult(() => CommonFailures.InvalidPurchaseId)
      from purchase in TryGetPurchase(receipt, purchaseId)
      let receiptWithUpdatedPurchase = receipt.UpdatePurchaseDetails(Update(purchase, patchModel))
      from version in _receiptRepository.SaveAsync(receiptWithUpdatedPurchase, cancellationToken)
      select receiptWithUpdatedPurchase.WithVersion(version).ClearChanges());
  }

  private static Result<Purchase> TryGetPurchase(Receipt receipt, Id purchaseId)
  {
    Purchase purchase = receipt.Purchases.SingleOrDefault(p => p.Id == purchaseId);
    return purchase != default
      ? Success.From(purchase)
      : Fail.OfType<Purchase>(Failure.NotFound("Purchase not found.", purchaseId.Value));
  }

  private static Purchase Update(Purchase purchase, PurchasePatchModel patchModel)
  {
    return purchase with
    {
      Item = patchModel.Item.OrElse(() => purchase.Item),
      Category = patchModel.Category.OrElse(() => purchase.Category),
      Quantity = patchModel.Quantity.OrElse(() => purchase.Quantity),
      UnitPrice = patchModel.UnitPrice.OrElse(() => purchase.UnitPrice),
      TotalDiscount = patchModel.TotalDiscount.OrElse(() => purchase.TotalDiscount),
      Description = patchModel.Description.OrElse(() => purchase.Description),
    };
  }
}
