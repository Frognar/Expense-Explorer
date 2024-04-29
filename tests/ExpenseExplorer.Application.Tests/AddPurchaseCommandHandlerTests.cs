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
    var result = await HandleValid(command);

    result.Id.Value.Should().Be(command.ReceiptId);
    result.Version.Value.Should().BeGreaterThan(0);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task ReturnsFailureWhenReceiptNotFound(AddPurchaseCommand command)
  {
    var result = await Handle(command with { ReceiptId = "invalid-Id" });
    var failure = result.Match(e => e, _ => throw new UnreachableException());
    failure.Message.Should().Contain("Receipt not found");
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task AddsPurchaseToReceipt(AddPurchaseCommand command)
  {
    (await HandleValid(command))
      .Purchases.Should()
      .NotBeEmpty();
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task SavesReceiptWhenValidCommand(AddPurchaseCommand command)
  {
    var receipt = await HandleValid(command);
    _receiptRepository.Should().Contain(r => r.Id == receipt.Id && r.Purchases.Count > 0);
  }

  [Property(Arbitrary = [typeof(InvalidAddPurchaseCommandGenerator)])]
  public async Task ReturnsFailureWhenRequestIsInvalid(AddPurchaseCommand command)
  {
    var result = await Handle(command);
    var failure = result.Match(e => e, _ => throw new UnreachableException());
    failure.Should().NotBeNull();
  }

  private async Task<Receipt> HandleValid(AddPurchaseCommand command)
  {
    var result = await Handle(command);
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
      Add(Receipt.Recreate([createFact], default));
    }

    public Task<Result<Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      this[0] = receipt.WithVersion(Version.Create(receipt.Version.Value + 1));
      return Task.FromResult(Success.From(Version.Create(receipt.Version.Value + 1)));
    }

    public Task<Result<Receipt>> GetAsync(Id id, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Receipt? receipt = this.SingleOrDefault(r => r.Id == id);
      return receipt is null
        ? Task.FromResult(Fail.OfType<Receipt>(new NotFoundFailure("Receipt not found", id.Value)))
        : Task.FromResult(Success.From(receipt));
    }
  }
}
