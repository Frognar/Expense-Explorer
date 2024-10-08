using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed class AddPurchaseCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<AddPurchaseCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(AddPurchaseCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    DbReceipt? receiptHeader = _context.Receipts.FirstOrDefault(r => r.Id == command.ReceiptId);
    if (receiptHeader is null)
    {
      throw new InvalidOperationException($"Receipt with id {command.ReceiptId} not found.");
    }

    DbPurchase purchase = new(
      command.ReceiptId,
      command.PurchaseId,
      command.Item,
      command.Category,
      command.Quantity,
      command.UnitPrice,
      command.TotalDiscount,
      command.Description);

    receiptHeader.Total += (purchase.Quantity * purchase.UnitPrice) - purchase.TotalDiscount;
    _context.Purchases.Add(purchase);
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
