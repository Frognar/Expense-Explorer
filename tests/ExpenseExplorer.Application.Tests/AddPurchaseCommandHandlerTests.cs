namespace ExpenseExplorer.Application.Tests;

using System.Collections.ObjectModel;
using System.Diagnostics;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public class AddPurchaseCommandHandlerTests
{
  private readonly FakeReceiptRepository _receiptRepository = new();

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task CanHandleValidCommand(AddPurchaseCommand command)
  {
    Receipt result = await HandleValid(command);

    result.Id.Value.Should().Be(command.ReceiptId);
    result.Version.Value.Should().BeGreaterThan(0);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task ReturnsFailureWhenReceiptNotFound(AddPurchaseCommand command)
  {
    Result<Receipt> result = await Handle(command with { ReceiptId = "invalid-Id" });
    Failure failure = result.Match(e => e, _ => throw new UnreachableException());
    failure.Message.Should().Contain("Receipt not found");
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task AddsPurchaseToReceipt(AddPurchaseCommand command)
  {
    (await HandleValid(command))
      .Purchases.Should()
      .NotBeEmpty();
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    AddPurchaseCommand command = new("receiptId", "item", "category", 1, 1, 0, null);

    Receipt receipt = await HandleValid(command);

    _receiptRepository.Should().Contain(r => r.Id == receipt.Id && r.Purchases.Count == 1);
  }

  [Property(Arbitrary = [typeof(InvalidAddPurchaseCommandGenerator)])]
  public async Task ReturnsFailureWhenRequestIsInvalid(AddPurchaseCommand command)
  {
    Result<Receipt> result = await Handle(command);
    Failure failure = result.Match(e => e, _ => throw new UnreachableException());
    failure.Should().NotBeNull();
  }

  private async Task<Receipt> HandleValid(AddPurchaseCommand command)
  {
    Result<Receipt> result = await Handle(command);
    return result.Match(_ => throw new UnreachableException(), r => r);
  }

  private async Task<Result<Receipt>> Handle(AddPurchaseCommand command)
  {
    AddPurchaseCommandHandler handler = new(_receiptRepository);
    return await handler.HandleAsync(command);
  }

  private sealed class FakeReceiptRepository : Collection<Receipt>, IReceiptRepository
  {
    public FakeReceiptRepository()
    {
      DateOnly today = new DateOnly(2000, 1, 1);
      ReceiptCreated createFact = new("receiptId", "store", today, today);
      Add(Receipt.Recreate([createFact], Version.Create(1UL)));
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
}
