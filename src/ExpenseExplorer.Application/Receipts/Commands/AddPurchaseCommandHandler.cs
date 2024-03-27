namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public class AddPurchaseCommandHandler(IReceiptRepository repository)
{
  private readonly IReceiptRepository repository = repository;

  public async Task<Either<Failure, Receipt>> HandleAsync(
    AddPurchaseCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Validated<Purchase> validated = PurchaseValidator.Validate(command);
    Either<Failure, Purchase> purchase = validated.ToEither();
    Either<Failure, Receipt> receipt = await repository.GetAsync(Id.Create(command.ReceiptId), cancellationToken);
    return await purchase
      .FlatMapRight(e => receipt.MapRight(r => r.AddPurchase(e)))
      .FlatMapRight(r => Save(r, cancellationToken));
  }

  private async Task<Either<Failure, Receipt>> Save(Receipt receipt, CancellationToken cancellationToken)
  {
    var result = await repository.Save(receipt, cancellationToken);
    return result.MapRight(_ => receipt.ClearChanges());
  }
}
