namespace ExpenseExplorer.Application.Receipts.Persistence;

using DotResult;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public interface IReceiptRepository
{
  Task<Result<Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken);

  Task<Result<Receipt>> GetAsync(Id id, CancellationToken cancellationToken);
}
