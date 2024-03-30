namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Exceptions;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public sealed class EventStoreReceiptRepository(string connectionString) : IReceiptRepository, IDisposable
{
  private readonly EventStoreWrapper _eventStore = new(connectionString);

  public void Dispose()
  {
    _eventStore.Dispose();
  }

  public async Task<Either<Failure, Unit>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(receipt);
      await _eventStore.SaveEvents(receipt.Id, receipt.UnsavedUnsavedChanges, cancellationToken);
      return Right.From<Failure, Unit>(Unit.Instance);
    }
    catch (EventSaveException ex)
    {
      return Left.From<Failure, Unit>(new FatalFailure(ex));
    }
  }

  public async Task<Either<Failure, Receipt>> GetAsync(Id id, CancellationToken cancellationToken)
  {
    try
    {
      List<Fact> events = (await _eventStore.GetEvents(id, cancellationToken)).ToList();
      return events.Count == 0
        ? Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", id))
        : Right.From<Failure, Receipt>(Receipt.Recreate(events));
    }
    catch (EventReadException ex)
    {
      return Left.From<Failure, Receipt>(new FatalFailure(ex));
    }
  }
}
