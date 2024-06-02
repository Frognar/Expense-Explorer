namespace ExpenseExplorer.GUI.Helpers;

using ExpenseExplorer.API.Contract.ReadModel;
using ExpenseExplorer.GUI.Data;

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
}
