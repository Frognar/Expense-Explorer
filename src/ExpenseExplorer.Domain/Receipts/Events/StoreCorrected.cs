namespace ExpenseExplorer.Domain.Receipts.Events;

using ExpenseExplorer.Domain.ValueObjects;

public class StoreCorrected : Fact
{
  public StoreCorrected(Id id, Store store)
  {
    Id = id;
    Store = store;
  }

  public Id Id { get; }

  public Store Store { get; }
}
