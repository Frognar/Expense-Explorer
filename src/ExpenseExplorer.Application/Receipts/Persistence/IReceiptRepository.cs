namespace ExpenseExplorer.Application.Receipts.Persistence;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public interface IReceiptRepository
{
  Task<Result<Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken);

  Task<Result<Receipt>> GetAsync(Id id, CancellationToken cancellationToken);
}
