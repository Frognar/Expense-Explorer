namespace ExpenseExplorer.ReadModel.Queries;

using System.Data.Common;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

public sealed class GetStoresQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetStoresQuery, Result<PageOf<Store>>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<PageOf<Store>>> HandleAsync(
    GetStoresQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      IQueryable<string> storeQuery = _context.Receipts.AsNoTracking()
        .OrderByMany(
          Order.DescendingBy<DbReceipt>(r => r.PurchaseDate),
          Order.DescendingBy<DbReceipt>(r => r.Id))
        .Select(r => r.Store)
        .Distinct();

      int totalCount = await storeQuery.CountAsync(cancellationToken);
      storeQuery = storeQuery.WhereContains(query.Search);
      int filteredCount = await storeQuery.CountAsync(cancellationToken);
      List<Store> storeList = await storeQuery
        .GetPage(query.PageNumber, query.PageSize)
        .Select(name => new Store(name))
        .ToListAsync(cancellationToken);

      PageOf<Store> page = Page.Of(storeList, totalCount, filteredCount, query.PageSize, query.PageNumber);
      return Success.From(page);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<Store>>(FailureFactory.Fatal(ex));
    }
  }
}
