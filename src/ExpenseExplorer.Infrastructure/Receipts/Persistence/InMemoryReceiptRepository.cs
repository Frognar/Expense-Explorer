namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public class InMemoryReceiptRepository : IReceiptRepository
{
  private readonly InMemoryEventStore eventStore = new();

  public async Task<Either<Failure, Unit>> Save(Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    await eventStore.SaveEvents(receipt.Id, receipt.UnsavedChanges);
    return Right.From<Failure, Unit>(Unit.Instance);
  }

  public Task<Either<Failure, Receipt>> GetAsync(Id id)
  {
    return Task.FromResult(Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", id)));
  }
}
