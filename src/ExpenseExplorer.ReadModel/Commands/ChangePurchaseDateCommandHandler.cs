namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

public sealed class ChangePurchaseDateCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<ChangePurchaseDateCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(ChangePurchaseDateCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    ArgumentNullException.ThrowIfNull(command);
    DbReceipt? dbReceipt = _context.Receipts.FirstOrDefault(r => r.Id == command.ReceiptId);
    if (dbReceipt is null)
    {
      throw new InvalidOperationException($"Receipt with id {command.ReceiptId} not found.");
    }

    dbReceipt.PurchaseDate = command.PurchaseDate;
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
