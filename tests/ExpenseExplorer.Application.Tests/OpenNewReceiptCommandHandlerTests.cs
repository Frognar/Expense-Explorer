using ExpenseExplorer.Tests.Common;

namespace ExpenseExplorer.Application.Tests;

public class OpenNewReceiptCommandHandlerTests
{
  private readonly FakeReceiptRepository _receiptRepository = new();

  [Property(Arbitrary = [typeof(ValidOpenNewReceiptCommandGenerator)])]
  public async Task CanHandleValidCommand(OpenNewReceiptCommand command)
  {
    Receipt receipt = await HandleValid(command);
    receipt.Store.Name.Should().Be(command.StoreName.Trim());
    receipt.PurchaseDate.Date.Should().Be(command.PurchaseDate);
    receipt.Version.Value.Should().Be(0);
  }

  [Property(Arbitrary = [typeof(InvalidOpenNewReceiptCommandGenerator)])]
  public async Task CanHandleInvalidCommand(OpenNewReceiptCommand command)
  {
    Failure failure = await HandleInvalid(command);
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, _) => throw new InvalidOperationException("Unexpected not found failure"),
      (_, errors) => errors.Should().NotBeEmpty());
  }

  [Property(Arbitrary = [typeof(ValidOpenNewReceiptCommandGenerator)])]
  public async Task SavesReceiptWhenValidCommand(OpenNewReceiptCommand command)
  {
    Receipt receipt = await HandleValid(command);
    _receiptRepository.Should().Contain(r => r.Id == receipt.Id);
  }

  private async Task<Failure> HandleInvalid(OpenNewReceiptCommand command)
    => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Receipt> HandleValid(OpenNewReceiptCommand command)
    => (await Handle(command)).Match(_ => throw new UnreachableException(), r => r);

  private async Task<Result<Receipt>> Handle(OpenNewReceiptCommand command)
    => await new OpenNewReceiptCommandHandler(_receiptRepository).HandleAsync(command);
}
