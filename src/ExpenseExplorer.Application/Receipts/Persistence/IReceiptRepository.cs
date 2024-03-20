namespace ExpenseExplorer.Application.Receipts.Persistence;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public interface IReceiptRepository
{
  Task Save(Receipt receipt);

  Task<Receipt?> GetAsync(Id id);
}
