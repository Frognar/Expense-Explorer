namespace ExpenseExplorer.Application.Tests;

using ExpenseExplorer.Tests.Common;

public class RemovePurchaseCommandHandlerTests
{
  private readonly FakeReceiptRepository _receiptRepository =
  [
    Receipt.Recreate(
        [
          new ReceiptCreated("receiptId", "store", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
          new PurchaseAdded("receiptId", "purchaseId", "i", "c", 1, 1, 0, string.Empty),
        ],
        Version.Create(1UL))
      .Match(_ => throw new UnreachableException(), r => r)
  ];

  [Fact]
  public async Task RemovesPurchaseFromReceipt()
  {
    RemovePurchaseCommand command = new("receiptId", "purchaseId");

    Receipt result = await HandleValid(command);

    result.Id.Value.Should().Be(command.ReceiptId);
    result.Purchases.Should().BeEmpty();
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    RemovePurchaseCommand command = new("receiptId", "purchaseId");

    _ = await HandleValid(command);

    Receipt receipt = _receiptRepository.Single(r => r.Id.Value == command.ReceiptId);
    receipt.Version.Value.Should().Be(2UL);
    receipt.Purchases.Should().BeEmpty();
  }

  [Fact]
  public async Task ReturnsFailureWhenReceiptNotFound()
  {
    RemovePurchaseCommand command = new("invalid-Id", "purchaseId");

    Failure failure = await HandleInvalid(command);

    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, id) => id.Should().Be("invalid-Id"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  private async Task<Failure> HandleInvalid(RemovePurchaseCommand command)
    => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Receipt> HandleValid(RemovePurchaseCommand command)
    => (await Handle(command)).Match(_ => throw new UnreachableException(), r => r);

  private async Task<Result<Receipt>> Handle(RemovePurchaseCommand command)
    => await new RemovePurchaseCommandHandler(_receiptRepository).HandleAsync(command);
}
