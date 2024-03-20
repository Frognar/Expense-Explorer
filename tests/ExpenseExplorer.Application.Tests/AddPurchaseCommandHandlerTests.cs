namespace ExpenseExplorer.Application.Tests;

using System.Collections.ObjectModel;
using System.Diagnostics;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class AddPurchaseCommandHandlerTests
{
  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task CanHandleValidCommand(AddPurchaseCommand command)
  {
    AddPurchaseCommandCommandHandler handler = new(new FakeReceiptRepository());
    var result = await handler.HandleAsync(command);
    result.Should().NotBeNull();
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task ReturnsFailureWhenReceiptNotFound(AddPurchaseCommand command)
  {
    AddPurchaseCommandCommandHandler handler = new(new FakeReceiptRepository());
    var result = await handler.HandleAsync(command with { ReceiptId = "invalid-Id" });
    Failure failure = result.Match(e => e, _ => throw new UnreachableException());
    failure.Message.Should().Contain("not found");
  }

  private sealed class FakeReceiptRepository : Collection<Receipt>, IReceiptRepository
  {
    public FakeReceiptRepository()
    {
      ReceiptCreated createEvent = new(
        Id.Create("receiptId"),
        Store.Create("store"),
        PurchaseDate.Create(TodayDateOnly, TodayDateOnly));

      Add(Receipt.Recreate([createEvent]));
    }

    public Task Save(Receipt receipt)
    {
      Add(receipt);
      return Task.CompletedTask;
    }

    public Task<Receipt?> GetAsync(Id id)
    {
      return Task.FromResult(this.SingleOrDefault(r => r.Id == id));
    }
  }
}
