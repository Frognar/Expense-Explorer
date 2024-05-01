namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public sealed class UpdatePurchaseDetailsCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<UpdatePurchaseDetailsCommand, Result<Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Receipt>> HandleAsync(
    UpdatePurchaseDetailsCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Result<Receipt> resultOfReceipt
      = await _receiptRepository.GetAsync(Id.Create(command.ReceiptId), cancellationToken);

    return resultOfReceipt.FlatMap(
      r => r.Purchases.Any(p => p.Id == Id.Create(command.PurchaseId))
        ? Success.From(r)
        : Fail.OfType<Receipt>(Failure.NotFound("Purchase not found", command.PurchaseId)));
  }
}
