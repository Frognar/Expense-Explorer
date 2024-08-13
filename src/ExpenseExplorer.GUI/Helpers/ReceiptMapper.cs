using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Contract.ReadModel;
using ExpenseExplorer.GUI.Data;

namespace ExpenseExplorer.GUI.Helpers;

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

  public static OpenNewReceiptRequest ToAddRequest(this ReceiptModel receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    return new OpenNewReceiptRequest(receipt.Store, receipt.PurchaseDate);
  }

  public static UpdateReceiptRequest ToEditRequest(this ReceiptModel receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    return new UpdateReceiptRequest(receipt.Store, receipt.PurchaseDate);
  }
}
