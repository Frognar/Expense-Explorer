namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteReceiptCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<DeleteReceiptCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(DeleteReceiptCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    await _context.Receipts.Where(r => r.Id == command.ReceiptId).ExecuteDeleteAsync(cancellationToken);
    return Unit.Instance;
  }
}
