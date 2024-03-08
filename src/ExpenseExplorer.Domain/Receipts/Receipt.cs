namespace ExpenseExplorer.Domain.Receipts;

using ExpenseExplorer.Domain.ValueObjects;

public class Receipt
{
  public Receipt(Id id, Store store, PurchaseDate purchaseDate)
  {
    Id = id;
    Store = store;
    PurchaseDate = purchaseDate;
  }

  public Id Id { get; }

  public Store Store { get; }

  public PurchaseDate PurchaseDate { get; }
}
