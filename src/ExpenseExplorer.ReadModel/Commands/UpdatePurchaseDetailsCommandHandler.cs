namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

public sealed class UpdatePurchaseDetailsCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<UpdatePurchaseDetailsCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(
    UpdatePurchaseDetailsCommand command,
    CancellationToken cancellationToken = default)
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
    dbPurchase.Item = command.Item;
    dbPurchase.Category = command.Category;
    dbPurchase.Quantity = command.Quantity;
    dbPurchase.UnitPrice = command.UnitPrice;
    dbPurchase.TotalDiscount = command.TotalDiscount;
    dbPurchase.Description = command.Description;

    DbPurchase purchase = new(
      command.ReceiptId,
      command.PurchaseId,
      command.Item,
      command.Category,
      command.Quantity,
      command.UnitPrice,
      command.TotalDiscount,
      command.Description);

    dbReceipt.Total += (purchase.Quantity * purchase.UnitPrice) - purchase.TotalDiscount;
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
