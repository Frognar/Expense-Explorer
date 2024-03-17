namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;

public class InMemoryReceiptRepository : IReceiptRepository
{
  private readonly InMemoryEventStore eventStore = new();

  public async Task Save(Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    await eventStore.SaveEvents(receipt.Id, receipt.UnsavedChanges);
  }
}
