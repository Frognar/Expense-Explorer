namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

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

  public async Task<Either<Failure, Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(receipt);
      Version version = await _eventStore.SaveEventsAsync(
        receipt.Id,
        receipt.Version,
        receipt.UnsavedUnsavedChanges,
        cancellationToken);

      return Right.From<Failure, Version>(version);
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
      (List<Fact> events, Version version) = await _eventStore.GetEventsAsync(id, cancellationToken);
      return events.Count == 0
        ? Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", id))
        : Right.From<Failure, Receipt>(Receipt.Recreate(events, version));
    }
    catch (EventReadException ex)
    {
      return Left.From<Failure, Receipt>(new FatalFailure(ex));
    }
  }
}
