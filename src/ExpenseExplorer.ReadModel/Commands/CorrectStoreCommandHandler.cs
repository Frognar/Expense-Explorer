namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;

public sealed class CorrectStoreCommandHandler : ICommandHandler<CorrectStoreCommand, Unit>
{
  private readonly ExpenseExplorerContext _context;

  public CorrectStoreCommandHandler(ExpenseExplorerContext context)
  {
    _context = context;
  }

  public async Task<Unit> HandleAsync(
    CorrectStoreCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    DbReceipt? dbReceipt = _context.Receipts.FirstOrDefault(r => r.Id == command.ReceiptId);
    if (dbReceipt is null)
    {
      throw new InvalidOperationException($"Receipt with id {command.ReceiptId} not found.");
    }

    dbReceipt.Store = command.Store;
    await _context.SaveChangesAsync(cancellationToken);
    return Unit.Instance;
  }
}
