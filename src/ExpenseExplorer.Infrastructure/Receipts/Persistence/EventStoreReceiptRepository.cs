namespace ExpenseExplorer.Infrastructure.Receipts.Persistence;

using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

#pragma warning disable CA1001
public class EventStoreReceiptRepository(string connectionString) : IReceiptRepository
#pragma warning restore CA1001
{
  private readonly EventStoreWrapper eventStore = new(connectionString);

  public async Task<Either<Failure, Unit>> Save(Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    await eventStore.SaveEvents(receipt.Id, receipt.UnsavedChanges);
    return Right.From<Failure, Unit>(Unit.Instance);
  }

  public async Task<Either<Failure, Receipt>> GetAsync(Id id)
  {
    List<Fact> events = (await eventStore.GetEvents(id)).ToList();
    return events.Count == 0
      ? Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", id))
      : Right.From<Failure, Receipt>(Receipt.Recreate(events));
  }
}
