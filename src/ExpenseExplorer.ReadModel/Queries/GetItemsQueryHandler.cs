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
      IQueryable<string> itemNames = _context.Purchases.AsNoTracking()
        .OrderByMany(Order.DescendingBy<DbPurchase>(p => p.PurchaseId))
        .Select(p => p.Item)
        .Distinct();

      int totalCount = await itemNames.CountAsync(cancellationToken);
      List<Item> itemList = await itemNames
        .WhereContains(query.Search)
        .GetPage(query.PageNumber, query.PageSize)
        .Select(name => new Item(name))
        .ToListAsync(cancellationToken);

      PageOf<Item> page = Page.Of(itemList, totalCount, query.PageSize, query.PageNumber);
      return Success.From(page);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<Item>>(Failure.Fatal(ex));
    }
  }
}
