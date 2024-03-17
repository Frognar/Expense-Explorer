namespace ExpenseExplorer.Application.Tests;

using System.Diagnostics;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;

public class OpenNewReceiptHandlerTests
{
  [Property(Arbitrary = [typeof(ValidOpenNewReceiptCommandGenerator)])]
  public async Task CanHandleValidCommand(OpenNewReceiptCommand command)
  {
    var receipt = (await Handle(command))
      .Match(_ => throw new UnreachableException(), receipt => receipt);

    receipt.Store.Name.Should().Be(command.StoreName.Trim());
    receipt.PurchaseDate.Date.Should().Be(command.PurchaseDate);
  }

  [Property(Arbitrary = [typeof(InvalidOpenNewReceiptCommandGenerator)])]
  public async Task CanHandleInvalidCommand(OpenNewReceiptCommand command)
  {
    var failure = (await Handle(command))
      .Match(failure => failure, _ => throw new UnreachableException());

    failure.Should().BeOfType<ValidationFailure>();
  }

  private static async Task<Either<Failure, Receipt>> Handle(OpenNewReceiptCommand command)
  {
    IReceiptRepository repository = new InMemoryReceiptRepository();
    OpenNewReceiptCommandHandler handler = new(repository);
    return await handler.HandleAsync(command);
  }
}
