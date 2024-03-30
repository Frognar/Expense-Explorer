namespace ExpenseExplorer.Application.Receipts.Persistence;

using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public interface IReceiptRepository
{
  Task<Either<Failure, Unit>> SaveAsync(Receipt receipt, CancellationToken cancellationToken);

  Task<Either<Failure, Receipt>> GetAsync(Id id, CancellationToken cancellationToken);
}
