namespace ExpenseExplorer.Infrastructure.Receipts.Projections.FactHandlers;

using System.Diagnostics.CodeAnalysis;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Infrastructure.Receipts.Projections.Models;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by DI container")]
internal sealed class ReceiptCreatedHandler(ExpenseExplorerContext context)
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task HandleAsync(ReceiptCreated fact, CancellationToken cancellationToken)
  {
    DbReceiptHeader receipt = new(fact.Id.Value, fact.Store.Name, fact.PurchaseDate.Date, 0);
    _context.ReceiptHeaders.Add(receipt);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
