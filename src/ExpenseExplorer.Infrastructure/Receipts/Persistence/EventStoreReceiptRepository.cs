namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using DotResult;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Infrastructure.Exceptions;
using FunctionalCore.Failures;

public sealed class EventStoreReceiptRepository(string connectionString) : IReceiptRepository, IDisposable
{
  private readonly EventStoreWrapper _eventStore = new(connectionString);

  public void Dispose()
  {
    _eventStore.Dispose();
  }

  public async Task<Result<Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(receipt);
      Version version = await _eventStore.SaveEventsAsync(
        receipt.Id,
        receipt.Version,
        receipt.UnsavedChanges,
        cancellationToken);

      return Success.From(version);
    }
    catch (FactSaveException ex)
    {
      return Fail.OfType<Version>(FailureFactory.Fatal(ex));
    }
  }

  public async Task<Result<Receipt>> GetAsync(Id id, CancellationToken cancellationToken)
  {
    try
    {
      (List<Fact> facts, Version version) = await _eventStore.GetEventsAsync(id, cancellationToken);
      return facts.Count == 0
        ? Fail.OfType<Receipt>(FailureFactory.NotFound("Receipt not found", id.Value))
        : Receipt.Recreate(facts, version);
    }
    catch (FactReadException ex)
    {
      return Fail.OfType<Receipt>(FailureFactory.Fatal(ex));
    }
  }
}
