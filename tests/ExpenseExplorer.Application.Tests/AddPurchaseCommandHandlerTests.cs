namespace ExpenseExplorer.Application.Tests;

using System.Collections.ObjectModel;
using System.Diagnostics;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class AddPurchaseCommandHandlerTests
{
  private readonly FakeReceiptRepository repository = new();

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task CanHandleValidCommand(AddPurchaseCommand command)
  {
    var result = await Handle(command);
    var receipt = result.Match(_ => throw new UnreachableException(), r => r);
    receipt.Id.Value.Should().Be(command.ReceiptId);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task ReturnsFailureWhenReceiptNotFound(AddPurchaseCommand command)
  {
    var result = await Handle(command with { ReceiptId = "invalid-Id" });
    var failure = result.Match(e => e, _ => throw new UnreachableException());
    failure.Message.Should().Contain("Receipt not found");
  }

  private async Task<Either<Failure, Receipt>> Handle(AddPurchaseCommand command)
  {
    AddPurchaseCommandCommandHandler handler = new(repository);
    return await handler.HandleAsync(command);
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

    public Task<Either<Failure, Unit>> Save(Receipt receipt)
    {
      return Task.FromResult(Right.From<Failure, Unit>(Unit.Instance));
    }

    public Task<Either<Failure, Receipt>> GetAsync(Id id)
    {
      Receipt? receipt = this.SingleOrDefault(r => r.Id == id);
      return receipt is null
        ? Task.FromResult(Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", id)))
        : Task.FromResult(Right.From<Failure, Receipt>(receipt));
    }
  }
}
