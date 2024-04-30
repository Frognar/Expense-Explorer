namespace ExpenseExplorer.Application.Tests;

using System.Diagnostics;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Domain.Receipts;
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
  public async Task AddsPurchaseToReceipt(AddPurchaseCommand command)
  {
    Receipt result = await HandleValid(command);
    result.Purchases.Should().NotBeEmpty();
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    Receipt receipt = await HandleValid(new AddPurchaseCommand("receiptId", "item", "category", 1, 1, 0, null));
    _receiptRepository.Should().Contain(r => r.Id == receipt.Id && r.Purchases.Count == 1);
  }

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task ReturnsFailureWhenReceiptNotFound(AddPurchaseCommand command)
  {
    Failure failure = await HandleInvalid(command with { ReceiptId = "invalid-Id" });
    failure.Should().BeOfType<NotFoundFailure>();
  }

  [Property(Arbitrary = [typeof(InvalidAddPurchaseCommandGenerator)])]
  public async Task ReturnsFailureWhenRequestIsInvalid(AddPurchaseCommand command)
  {
    Failure failure = await HandleInvalid(command);
    failure.Should().NotBeNull();
  }

  private async Task<Failure> HandleInvalid(AddPurchaseCommand command)
    => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Receipt> HandleValid(AddPurchaseCommand command)
    => (await Handle(command)).Match(_ => throw new UnreachableException(), r => r);

  private async Task<Result<Receipt>> Handle(AddPurchaseCommand command)
    => await new AddPurchaseCommandHandler(_receiptRepository).HandleAsync(command);
}
