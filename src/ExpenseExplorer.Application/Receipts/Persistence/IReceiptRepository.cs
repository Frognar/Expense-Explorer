namespace ExpenseExplorer.Application.Receipts.Persistence;

using ExpenseExplorer.Domain.Receipts;

public interface IReceiptRepository
{
  Task Save(Receipt receipt);
}
