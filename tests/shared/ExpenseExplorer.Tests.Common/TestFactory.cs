using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Tests.Common;

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

  public static NonFutureDate PurchaseDate(DateOnly date)
  {
    return NonFutureDate.TryCreate(date, date).ForceValue();
  }
}
