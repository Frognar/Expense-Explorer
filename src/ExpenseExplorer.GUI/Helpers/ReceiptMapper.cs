namespace ExpenseExplorer.GUI.Helpers;

using ExpenseExplorer.API.Contract.ReadModel;
using ExpenseExplorer.GUI.Data;

public static class ReceiptMapper
{
  public static ReceiptModel ToViewModel(this ReceiptHeaderResponse receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    return new ReceiptModel
    {
      Id = receipt.Id, Store = receipt.Store, PurchaseDate = receipt.PurchaseDate, Total = receipt.Total,
    };
  }
}
