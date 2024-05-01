namespace ExpenseExplorer.Application.Tests;

using System.Diagnostics;
using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Domain.Receipts;
using FunctionalCore.Monads;

public class UpdatePurchaseDetailsCommandHandlerTests
{
  private readonly FakeReceiptRepository _receiptRepository = new();

  [Fact]
  public void CanCreateHandler()
  {
    UpdatePurchaseDetailsCommandHandler handler = new(_receiptRepository);
    handler.Should().BeAssignableTo<ICommandHandler<UpdatePurchaseDetailsCommand, Result<Receipt>>>();
  }

  [Fact]
  public async Task CanHandleValidCommand()
  {
    UpdatePurchaseDetailsCommand command = new(
      "receiptWithPurchaseId",
      "purchaseId",
      (string?)"item",
      (string?)"category",
      (decimal?)1m,
      (decimal?)1m,
      (decimal?)1m,
      (string?)"description");

    Result<Receipt> result = await new UpdatePurchaseDetailsCommandHandler(_receiptRepository).HandleAsync(command);
    Receipt receipt = result.Match(_ => throw new UnreachableException(), r => r);
    receipt.Id.Value.Should().Be(command.ReceiptId);
    receipt.Version.Value.Should().BeGreaterThan(0);
  }
}
