namespace ExpenseExplorer.Tests.Common;

using ExpenseExplorer.Domain.Receipts;

public static class TestFactory
{
  public static Receipt Receipt(string store, DateOnly date)
  {
    return Domain.Receipts.Receipt.New(Store(store), PurchaseDate(date), date);
  }

  public static Store Store(string store)
  {
    return Domain.ValueObjects.Store.TryCreate(store).ForceValue();
  }

  public static PurchaseDate PurchaseDate(DateOnly date)
  {
    return Domain.ValueObjects.PurchaseDate.TryCreate(date, date).ForceValue();
  }
}
