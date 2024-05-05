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
      IQueryable<string> storeNames = _context.Receipts.AsNoTracking()
        .OrderByMany(
          Order.DescendingBy<DbReceipt>(r => r.PurchaseDate),
          Order.DescendingBy<DbReceipt>(r => r.Id))
        .Select(r => r.Store)
        .Distinct();

      int totalCount = await storeNames.CountAsync(cancellationToken);
      List<Store> storeList = await storeNames
        .Filter(StoreContains(query.Search))
        .GetPage(query.PageNumber, query.PageSize)
        .Select(name => new Store(name))
        .ToListAsync(cancellationToken);

      PageOf<Store> page = Page.Of(storeList, totalCount, query.PageSize, query.PageNumber);
      return Success.From(page);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<Store>>(Failure.Fatal(ex));
    }
  }

  private static Expression<Func<string, bool>> StoreContains(string search)
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
    return storeName => storeName.ToUpper().Contains(search);
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }
}
