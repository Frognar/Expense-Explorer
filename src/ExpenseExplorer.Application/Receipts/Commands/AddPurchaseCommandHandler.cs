namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public class AddPurchaseCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<AddPurchaseCommand, Either<Failure, Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Either<Failure, Receipt>> HandleAsync(
    AddPurchaseCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Validated<Purchase> validated = PurchaseValidator.Validate(command);
    Either<Failure, Purchase> purchase = validated.ToEither();
    Either<Failure, Receipt> receipt = await _receiptRepository.GetAsync(
      Id.Create(command.ReceiptId),
      cancellationToken);

    return await purchase
      .FlatMapRight(e => receipt.MapRight(r => r.AddPurchase(e)))
      .FlatMapRightAsync(r => SaveAsync(r, cancellationToken));
  }

  private async Task<Either<Failure, Receipt>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    var result = await _receiptRepository.SaveAsync(receipt, cancellationToken);
    return result.MapRight(_ => receipt.ClearChanges());
  }
}
