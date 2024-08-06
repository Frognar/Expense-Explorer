namespace ExpenseExplorer.Application.Tests;

using System.Collections.ObjectModel;
using ExpenseExplorer.Application.Receipts.Persistence;
using FunctionalCore.Failures;

internal sealed class FakeReceiptRepository : Collection<Receipt>, IReceiptRepository
{
  public Task<Result<Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    Version version = Version.Create(receipt.Version.Value + (ulong)receipt.UnsavedChanges.Count());
    if (this.FirstOrDefault(r => r.Id == receipt.Id) is not null)
    {
      this[0] = receipt.WithVersion(version).ClearChanges();
    }
    else
    {
      Add(receipt.WithVersion(version).ClearChanges());
    }

    return Task.FromResult(Success.From(version));
  }

  public Task<Result<Receipt>> GetAsync(Id id, CancellationToken cancellationToken)
  {
    Receipt? receipt = this.SingleOrDefault(r => r.Id == id);
    return receipt is null
      ? Task.FromResult(Fail.OfType<Receipt>(FailureFactory.NotFound("Receipt not found", id.Value)))
      : Task.FromResult(Success.From(receipt));
  }
}
