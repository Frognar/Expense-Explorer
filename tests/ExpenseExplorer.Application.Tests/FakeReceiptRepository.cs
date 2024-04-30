namespace ExpenseExplorer.Application.Tests;

using System.Collections.ObjectModel;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

internal sealed class FakeReceiptRepository : Collection<Receipt>, IReceiptRepository
{
  public FakeReceiptRepository()
  {
    DateOnly today = new DateOnly(2000, 1, 1);
    ReceiptCreated createFact = new("receiptId", "store", today, today);
    Add(Receipt.Recreate([createFact], Version.Create(0UL)));
  }

  public Task<Result<Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    Version version = Version.Create(receipt.Version.Value + (ulong)receipt.UnsavedChanges.Count());
    this[0] = receipt.WithVersion(version).ClearChanges();
    return Task.FromResult(Success.From(version));
  }

  public Task<Result<Receipt>> GetAsync(Id id, CancellationToken cancellationToken)
  {
    Receipt? receipt = this.SingleOrDefault(r => r.Id == id);
    return receipt is null
      ? Task.FromResult(Fail.OfType<Receipt>(Failure.NotFound("Receipt not found", id.Value)))
      : Task.FromResult(Success.From(receipt));
  }
}
