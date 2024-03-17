namespace ExpenseExplorer.Application.Tests;

using System.Diagnostics;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;

public class OpenNewReceiptHandlerTests
{
  [Fact]
  public void CanCreateHandler()
  {
    IReceiptRepository repository = new InMemoryReceiptRepository();
    OpenNewReceiptCommandHandler handler = new(repository);
    handler.Should().NotBeNull();
  }

  [Property(Arbitrary = [typeof(ValidOpenNewReceiptCommandGenerator)])]
  public async Task CanHandleValidCommand(OpenNewReceiptCommand command)
  {
    IReceiptRepository repository = new InMemoryReceiptRepository();
    OpenNewReceiptCommandHandler handler = new(repository);

    var response = await handler.HandleAsync(command);
    var receipt = response.Match(_ => throw new UnreachableException(), receipt => receipt);

    receipt.Store.Name.Should().Be(command.StoreName.Trim());
    receipt.PurchaseDate.Date.Should().Be(command.PurchaseDate);
  }
}
