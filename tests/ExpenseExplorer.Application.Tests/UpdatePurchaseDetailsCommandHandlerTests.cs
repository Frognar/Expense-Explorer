namespace ExpenseExplorer.Application.Tests;

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
}
