namespace ExpenseExplorer.ReadModel.Queries;

using System.Data.Common;
using System.Linq.Expressions;
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
        .Filter(
          PurchaseDateBetween(query.After, query.Before),
          TotalBetween(query.MinTotal, query.MaxTotal),
          StoreContains(query.Search))
        .OrderByMany(
          Order.AscendingBy<DbReceiptHeader>(r => r.PurchaseDate),
          Order.DescendingBy<DbReceiptHeader>(r => r.Id))
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

  private static Expression<Func<DbReceiptHeader, bool>> PurchaseDateBetween(DateOnly min, DateOnly max)
  {
    return r => r.PurchaseDate >= min && r.PurchaseDate <= max;
  }

  private static Expression<Func<DbReceiptHeader, bool>> TotalBetween(decimal min, decimal max)
  {
    return r => r.Total >= min && r.Total <= max;
  }

  private static Expression<Func<DbReceiptHeader, bool>> StoreContains(string search)
  {
    if (string.IsNullOrWhiteSpace(search))
    {
      return r => true;
    }

    search = search.ToUpperInvariant();

    // EFCore does not support StringComparison and CultureInfo enums in LINQ queries
#pragma warning disable CA1304
#pragma warning disable CA1311
#pragma warning disable CA1862
    return r => r.Store.ToUpper().Contains(search);
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }
}
