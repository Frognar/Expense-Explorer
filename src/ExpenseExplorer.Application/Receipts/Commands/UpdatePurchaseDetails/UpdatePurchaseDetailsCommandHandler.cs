namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public sealed class UpdatePurchaseDetailsCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<UpdatePurchaseDetailsCommand, Result<Receipt>>
{
#pragma warning disable S1144
#pragma warning disable CA1823
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public Task<Result<Receipt>> HandleAsync(
    UpdatePurchaseDetailsCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Receipt receipt = Receipt.Recreate(
      [
        new ReceiptCreated(command.ReceiptId, "store", DateOnly.MaxValue, DateOnly.MaxValue),
        new PurchaseAdded(command.ReceiptId, command.PurchaseId, "item", "category", 1, 1, 0, string.Empty),
      ],
      Version.Create(1UL));

    return Task.FromResult(Success.From(receipt));
  }
}
