namespace ExpenseExplorer.ReadModel.Commands;

using System.Diagnostics.CodeAnalysis;
using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by DI container")]
public sealed class CreateReceiptCommandHandler(ExpenseExplorerContext context)
  : ICommandHandler<CreateReceiptCommand, Unit>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Unit> HandleAsync(CreateReceiptCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    _context.ReceiptHeaders.Add(new DbReceiptHeader(command.Id, command.Store, command.PurchaseDate, 0));
    await _context.SaveChangesAsync(cancellationToken);
    Console.WriteLine(command);
    return Unit.Instance;
  }
}
