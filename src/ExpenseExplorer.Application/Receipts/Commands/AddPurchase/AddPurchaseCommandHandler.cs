namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public class AddPurchaseCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<AddPurchaseCommand, Result<Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Receipt>> HandleAsync(
    AddPurchaseCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Result<Receipt> resultOfReceipt = await (
      from purchase in AddPurchaseValidator.Validate(command)
      from receipt in Id.TryCreate(command.ReceiptId)
        .ToResult(() => CommonFailures.InvalidReceiptId)
        .FlatMapAsync(async id => await _receiptRepository.GetAsync(id, cancellationToken))
      select receipt.AddPurchase(purchase));

    return await (
      from receipt in resultOfReceipt
      from version in _receiptRepository.SaveAsync(receipt, cancellationToken)
      select receipt.WithVersion(version).ClearChanges());
  }
}
