namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using ExpenseExplorer.Application.Exceptions;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

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
        receipt.UnsavedChanges,
        cancellationToken);

      return Right.From<Failure, Version>(version);
    }
    catch (FactSaveException ex)
    {
      return Left.From<Failure, Version>(new FatalFailure(ex));
    }
  }

  public async Task<Either<Failure, Receipt>> GetAsync(Id id, CancellationToken cancellationToken)
  {
    try
    {
      (List<Fact> facts, Version version) = await _eventStore.GetEventsAsync(id, cancellationToken);
      return facts.Count == 0
        ? Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", id.Value))
        : Right.From<Failure, Receipt>(Receipt.Recreate(facts, version));
    }
    catch (FactReadException ex)
    {
      return Left.From<Failure, Receipt>(new FatalFailure(ex));
    }
  }
}
