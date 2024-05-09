namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;
using Microsoft.EntityFrameworkCore;

public sealed class RemovePurchaseCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<RemovePurchaseCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(RemovePurchaseCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    await _context.Purchases
      .Where(p => p.ReceiptId == command.ReceiptId && p.PurchaseId == command.PurchaseId)
      .ExecuteDeleteAsync(cancellationToken);

    return Unit.Instance;
  }
}
