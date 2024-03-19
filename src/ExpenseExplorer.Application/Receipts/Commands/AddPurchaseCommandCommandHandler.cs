namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.ValueObjects;

public class AddPurchaseCommandCommandHandler
{
#pragma warning disable CA1822
  public Task<object> HandleAsync(AddPurchaseCommand command)
#pragma warning restore CA1822
  {
    ArgumentNullException.ThrowIfNull(command);
    Validated<Purchase> validated = PurchaseValidator.Validate(command);
    return Task.FromResult((object)validated);
  }
}
