namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Exceptions;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class InMemoryReceiptRepository : IReceiptRepository
{
  public async Task<Either<Failure, Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    try
    {
      cancellationToken.ThrowIfCancellationRequested();
      ArgumentNullException.ThrowIfNull(receipt);
      await InMemoryEventStore.SaveEventsAsync(receipt.Id, receipt.UnsavedUnsavedChanges);
      return Right.From<Failure, Version>(receipt.Version.Next());
    }
    catch (EventSaveException ex)
    {
      return Left.From<Failure, Version>(new FatalFailure(ex));
    }
  }

  public async Task<Either<Failure, Receipt>> GetAsync(Id id, CancellationToken cancellationToken)
  {
    try
    {
      cancellationToken.ThrowIfCancellationRequested();
      List<Fact> events = (await InMemoryEventStore.GetEventsAsync(id)).ToList();
      return events.Count == 0
        ? Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", id))
        : Right.From<Failure, Receipt>(Receipt.Recreate(events, Version.Create((ulong)events.Count)));
    }
    catch (EventReadException ex)
    {
      return Left.From<Failure, Receipt>(new FatalFailure(ex));
    }
  }
}
