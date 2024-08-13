using ExpenseExplorer.Tests.Common;

namespace ExpenseExplorer.Application.Tests;

public class AddPurchaseCommandHandlerTests
{
  private readonly FakeReceiptRepository _receiptRepository =
  [
    Receipt.Recreate(
        [new ReceiptCreated("receiptId", "store", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1))],
        Version.Create(0UL))
      .Match(_ => throw new UnreachableException(), r => r)
  ];

  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task AddsPurchaseToReceipt(AddPurchaseCommand command)
  {
    Receipt result = await HandleValid(command);
    result.Id.Value.Should().Be(command.ReceiptId);
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
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, id) => id.Should().Be("invalid-Id"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  [Property(Arbitrary = [typeof(InvalidAddPurchaseCommandGenerator)])]
  public async Task ReturnsFailureWhenInvalidCommand(AddPurchaseCommand command)
  {
    Failure failure = await HandleInvalid(command);
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, _) => throw new InvalidOperationException("Unexpected not found failure"),
      (_, errors) => errors.Should().NotBeEmpty());
  }

  private async Task<Failure> HandleInvalid(AddPurchaseCommand command)
    => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Receipt> HandleValid(AddPurchaseCommand command)
    => (await Handle(command)).Match(_ => throw new UnreachableException(), r => r);

  private async Task<Result<Receipt>> Handle(AddPurchaseCommand command)
    => await new AddPurchaseCommandHandler(_receiptRepository).HandleAsync(command);
}
