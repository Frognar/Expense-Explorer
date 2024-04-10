namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;
using Microsoft.EntityFrameworkCore;

public class GetReceiptQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetReceiptQuery, PageOf<ReceiptHeaders>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<PageOf<ReceiptHeaders>> HandleAsync(
    GetReceiptQuery query,
    CancellationToken cancellationToken = default)
  {
    var receipts = await _context.ReceiptHeaders.AsNoTracking()
      .OrderBy(r => r.PurchaseDate)
      .Skip(0)
      .Take(10)
      .Select(r => new ReceiptHeaders(r.Id, r.Store, r.PurchaseDate, r.Total))
      .ToListAsync(cancellationToken);

    int totalCount = await _context.ReceiptHeaders.CountAsync(cancellationToken);
    var response = Page.Of(receipts, totalCount);
    return response;
  }
}
