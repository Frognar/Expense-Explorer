namespace ExpenseExplorer.Application.Tests;

using ExpenseExplorer.Application.Receipts.Commands;

public class AddPurchaseCommandHandlerTests
{
  [Property(Arbitrary = [typeof(ValidAddPurchaseCommandGenerator)])]
  public async Task CanHandleValidCommand(AddPurchaseCommand command)
  {
    AddPurchaseCommandCommandHandler handler = new();
    var result = await handler.HandleAsync(command);
    result.Should().NotBeNull();
  }
}
