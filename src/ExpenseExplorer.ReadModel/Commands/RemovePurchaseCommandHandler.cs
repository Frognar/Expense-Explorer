namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

public sealed class RemovePurchaseCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<RemovePurchaseCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(RemovePurchaseCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    DbPurchase? dbPurchase = _context.Purchases
      .FirstOrDefault(p => p.ReceiptId == command.ReceiptId && p.PurchaseId == command.PurchaseId);

    if (dbPurchase is null)
    {
      return Unit.Instance;
    }

    _context.Purchases.Remove(dbPurchase);
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
