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
      let updatedPurchase = Update(purchase, patchModel)
      select receipt.UpdatePurchaseDetails(updatedPurchase));
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
      Item = patchModel.Item.Match(() => purchase.Item, item => item),
      Category = patchModel.Category.Match(() => purchase.Category, category => category),
      Quantity = patchModel.Quantity.Match(() => purchase.Quantity, quantity => quantity),
      UnitPrice = patchModel.UnitPrice.Match(() => purchase.UnitPrice, unitPrice => unitPrice),
      TotalDiscount = patchModel.TotalDiscount.Match(() => purchase.TotalDiscount, totalDiscount => totalDiscount),
      Description = patchModel.Description.Match(() => purchase.Description, description => description),
    };
  }
}
