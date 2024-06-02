namespace ExpenseExplorer.GUI.Data;

public sealed class PurchaseModel
{
  public string Id { get; set; } = string.Empty;

  public string Item { get; set; } = string.Empty;

  public string Category { get; set; } = string.Empty;

  public decimal Quantity { get; set; }

  public decimal UnitPrice { get; set; }

  public decimal TotalDiscount { get; set; }

  public decimal TotalCost => (UnitPrice * Quantity) - TotalDiscount;

  public string Description { get; set; } = string.Empty;

  public PurchaseModel MakeCopy()
  {
    return new PurchaseModel
    {
      Id = Id,
      Item = Item,
      Category = Category,
      Quantity = Quantity,
      UnitPrice = UnitPrice,
      TotalDiscount = TotalDiscount,
      Description = Description,
    };
  }

  public void CopyFrom(PurchaseModel purchase)
  {
    ArgumentNullException.ThrowIfNull(purchase);
    Item = purchase.Item;
    Category = purchase.Category;
    Quantity = purchase.Quantity;
    UnitPrice = purchase.UnitPrice;
    TotalDiscount = purchase.TotalDiscount;
    Description = purchase.Description;
  }
}
