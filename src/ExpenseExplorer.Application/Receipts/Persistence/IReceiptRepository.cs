using DotResult;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Application.Receipts.Persistence;

public interface IReceiptRepository
{
  Task<Result<Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken);

  Task<Result<Receipt>> GetAsync(Id id, CancellationToken cancellationToken);
}
