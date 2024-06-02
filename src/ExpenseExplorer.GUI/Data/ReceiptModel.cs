namespace ExpenseExplorer.GUI.Data;

public sealed class ReceiptModel
{
  public string Id { get; set; } = string.Empty;

  public string Store { get; set; } = string.Empty;

  public DateOnly PurchaseDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

  public decimal Total { get; set; }

  public ReceiptModel MakeCopy()
  {
    return new ReceiptModel
    {
      Id = Id, Store = Store, PurchaseDate = PurchaseDate, Total = Total,
    };
  }

  public void CopyFrom(ReceiptModel receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    Store = receipt.Store;
    PurchaseDate = receipt.PurchaseDate;
    Total = receipt.Total;
  }
}
