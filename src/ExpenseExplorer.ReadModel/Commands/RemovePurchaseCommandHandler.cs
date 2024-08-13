using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed class RemovePurchaseCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<RemovePurchaseCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(RemovePurchaseCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    DbReceipt? dbReceipt = _context.Receipts.FirstOrDefault(r => r.Id == command.ReceiptId);
    if (dbReceipt is null)
    {
      throw new InvalidOperationException($"Receipt with id {command.ReceiptId} not found.");
    }

    DbPurchase? dbPurchase = _context.Purchases.FirstOrDefault(r => r.PurchaseId == command.PurchaseId);
    if (dbPurchase is null)
    {
      throw new InvalidOperationException($"Purchase with id {command.PurchaseId} not found.");
    }

    dbReceipt.Total -= (dbPurchase.Quantity * dbPurchase.UnitPrice) - dbPurchase.TotalDiscount;
    _context.Purchases.Remove(dbPurchase);
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
