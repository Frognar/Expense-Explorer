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
        .Filter(ItemContains(query.Search))
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

  private static Expression<Func<string, bool>> ItemContains(string search)
  {
    if (string.IsNullOrWhiteSpace(search))
    {
      return s => true;
    }

    search = search.ToUpperInvariant();

    // EFCore does not support StringComparison and CultureInfo enums in LINQ queries
#pragma warning disable CA1304
#pragma warning disable CA1311
#pragma warning disable CA1862
    return itemName => itemName.ToUpper().Contains(search);
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }
}
