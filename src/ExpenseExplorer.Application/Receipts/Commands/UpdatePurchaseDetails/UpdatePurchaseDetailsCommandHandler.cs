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
      from purchase in TryGetPurchase(receipt, purchaseId)
      select receipt.UpdatePurchaseDetails(Update(purchase, CreatePatchModel(command))));
  }

  private static Result<Purchase> TryGetPurchase(Receipt receipt, Id purchaseId)
  {
    Purchase purchase = receipt.Purchases.SingleOrDefault(p => p.Id == purchaseId);
    return purchase != default
      ? Success.From(purchase)
      : Fail.OfType<Purchase>(Failure.NotFound("Purchase not found.", purchaseId.Value));
  }

  private static PurchasePatchModel CreatePatchModel(UpdatePurchaseDetailsCommand command)
  {
    Maybe<Item> item = string.IsNullOrWhiteSpace(command.Item) ? None.OfType<Item>() : Item.TryCreate(command.Item);
    Maybe<Category> category = string.IsNullOrWhiteSpace(command.Category)
      ? None.OfType<Category>()
      : Category.TryCreate(command.Category);

    Maybe<Quantity> quantity = command.Quantity.HasValue
      ? Quantity.TryCreate(command.Quantity.Value)
      : None.OfType<Quantity>();

    Maybe<Money> unitPrice
      = command.UnitPrice.HasValue ? Money.TryCreate(command.UnitPrice.Value) : None.OfType<Money>();

    Maybe<Money> totalDiscount = command.TotalDiscount.HasValue
      ? Money.TryCreate(command.TotalDiscount.Value)
      : None.OfType<Money>();

    Maybe<Description> description = string.IsNullOrWhiteSpace(command.Description)
      ? None.OfType<Description>()
      : Some.From(Description.Create(command.Description));

    return PurchasePatchModel.Create(
      item,
      category,
      quantity,
      unitPrice,
      totalDiscount,
      description);
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
