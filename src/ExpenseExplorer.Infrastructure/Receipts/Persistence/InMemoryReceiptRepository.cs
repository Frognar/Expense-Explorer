namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;

public class InMemoryReceiptRepository : IReceiptRepository
{
  public Task Save(Receipt receipt)
  {
    return Task.CompletedTask;
  }
}
