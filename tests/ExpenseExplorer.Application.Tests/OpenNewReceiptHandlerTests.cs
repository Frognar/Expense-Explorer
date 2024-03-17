namespace ExpenseExplorer.Application.Tests;

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
}
