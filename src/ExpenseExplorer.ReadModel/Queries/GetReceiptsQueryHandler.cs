namespace ExpenseExplorer.ReadModel.Queries;

using System.Data.Common;
using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using FunctionalCore.Monads;
using Microsoft.EntityFrameworkCore;

public class GetReceiptsQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetReceiptsQuery, Either<Failure, PageOf<ReceiptHeaders>>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Either<Failure, PageOf<ReceiptHeaders>>> HandleAsync(
    GetReceiptsQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      List<ReceiptHeaders> receipts = await _context.ReceiptHeaders.AsNoTracking()
        .OrderByMany(
          Order.AscendingBy<DbReceiptHeader>(r => r.PurchaseDate),
          Order.AscendingBy<DbReceiptHeader>(r => r.Id))
        .GetPage(query.PageNumber, query.PageSize)
        .Select(r => new ReceiptHeaders(r.Id, r.Store, r.PurchaseDate, r.Total))
        .ToListAsync(cancellationToken);

      int totalCount = await _context.ReceiptHeaders.CountAsync(cancellationToken);
      PageOf<ReceiptHeaders> response = Page.Of(receipts, totalCount, query.PageSize, query.PageNumber);
      return Right.From<Failure, PageOf<ReceiptHeaders>>(response);
    }
    catch (DbException ex)
    {
      return Left.From<Failure, PageOf<ReceiptHeaders>>(new FatalFailure(ex));
    }
  }
}
