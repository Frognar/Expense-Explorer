using System.Data.Common;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed class GetItemsQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetItemsQuery, Result<PageOf<Item>>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<PageOf<Item>>> HandleAsync(
    GetItemsQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      IQueryable<string> itemQuery = _context.Purchases.AsNoTracking()
        .OrderByMany(Order.DescendingBy<DbPurchase>(p => p.PurchaseId))
        .Select(p => p.Item)
        .Distinct();

      int totalCount = await itemQuery.CountAsync(cancellationToken);
      itemQuery = itemQuery.WhereContains(query.Search);
      int filteredCount = await itemQuery.CountAsync(cancellationToken);
      List<Item> itemList = await itemQuery
        .GetPage(query.PageNumber, query.PageSize)
        .Select(name => new Item(name))
        .ToListAsync(cancellationToken);

      PageOf<Item> page = Page.Of(itemList, totalCount, filteredCount, query.PageSize, query.PageNumber);
      return Success.From(page);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<Item>>(FailureFactory.Fatal(ex));
    }
  }
}
