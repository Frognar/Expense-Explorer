using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Contract.ReadModel;
using ExpenseExplorer.GUI.Data;

namespace ExpenseExplorer.GUI.Helpers;

public static class PurchaseMapper
{
  public static PurchaseModel ToViewModel(this GetReceiptPurchaseResponse purchase)
  {
    ArgumentNullException.ThrowIfNull(purchase);
    return new PurchaseModel
    {
      Id = purchase.Id,
      Item = purchase.Item,
      Category = purchase.Category,
      UnitPrice = purchase.UnitPrice,
      Quantity = purchase.Quantity,
      TotalDiscount = purchase.TotalDiscount,
      Description = purchase.Description,
    };
  }

  public static AddPurchaseRequest ToAddRequest(this PurchaseModel purchase)
  {
    ArgumentNullException.ThrowIfNull(purchase);
    return new AddPurchaseRequest(
      purchase.Item,
      purchase.Category,
      purchase.Quantity,
      purchase.UnitPrice,
      purchase.TotalDiscount,
      purchase.Description);
  }

  public static UpdatePurchaseRequest ToEditRequest(this PurchaseModel purchase)
  {
    ArgumentNullException.ThrowIfNull(purchase);
    return new UpdatePurchaseRequest(
      purchase.Item,
      purchase.Category,
      purchase.Quantity,
      purchase.UnitPrice,
      purchase.TotalDiscount,
      purchase.Description);
  }
}
