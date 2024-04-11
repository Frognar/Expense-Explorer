namespace ExpenseExplorer.Application.Receipts.Persistence;

using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public interface IReceiptRepository
{
  Task<Either<Failure, Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken);

  Task<Either<Failure, Receipt>> GetAsync(Id id, CancellationToken cancellationToken);
}
