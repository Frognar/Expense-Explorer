namespace ExpenseExplorer.Application.Tests;

using System.Diagnostics;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Tests.Common.Generators.Commands;
using FunctionalCore.Monads;

public class UpdatePurchaseDetailsCommandHandlerTests
{
  private readonly FakeReceiptRepository _receiptRepository = new();

  [Property(Arbitrary = [typeof(ValidUpdatePurchaseDetailsCommandGenerator)])]
  public async Task CanHandleValidCommand(UpdatePurchaseDetailsCommand command)
  {
    Result<Receipt> result = await new UpdatePurchaseDetailsCommandHandler(_receiptRepository).HandleAsync(command);
    Receipt receipt = result.Match(_ => throw new UnreachableException(), r => r);
    receipt.Id.Value.Should().Be(command.ReceiptId);
    receipt.Version.Value.Should().BeGreaterThan(0);
  }
}
