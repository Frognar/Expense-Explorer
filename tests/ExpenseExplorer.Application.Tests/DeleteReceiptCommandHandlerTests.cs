namespace ExpenseExplorer.Application.Tests;

using ExpenseExplorer.Tests.Common;
using FunctionalCore;

public class DeleteReceiptCommandHandlerTests
{
  private readonly FakeReceiptRepository _receiptRepository =
  [
    Receipt.Recreate(
        [new ReceiptCreated("receiptId", "store", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1))],
        Version.Create(0UL))
      .Match(_ => throw new UnreachableException(), r => r)
  ];

  [Fact]
  public async Task DeletesReceipt()
  {
    DeleteReceiptCommand command = new("receiptId");

    Unit result = await HandleValid(command);

    result.Should().Be(Unit.Instance);
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    DeleteReceiptCommand command = new("receiptId");

    _ = await HandleValid(command);

    Receipt receipt = _receiptRepository.Single(r => r.Id.Value == command.ReceiptId);
    receipt.Version.Value.Should().Be(1UL);
  }

  [Fact]
  public async Task ReturnsFailureWhenReceiptNotFound()
  {
    DeleteReceiptCommand command = new("invalid-Id");

    Failure failure = await HandleInvalid(command);

    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, id) => id.Should().Be("invalid-Id"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  private async Task<Failure> HandleInvalid(DeleteReceiptCommand command)
    => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Unit> HandleValid(DeleteReceiptCommand command)
    => (await Handle(command)).Match(_ => throw new UnreachableException(), u => u);

  private async Task<Result<Unit>> Handle(DeleteReceiptCommand command)
    => await new DeleteReceiptCommandHandler(_receiptRepository).HandleAsync(command);
}
