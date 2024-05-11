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

public sealed class GetCategoriesQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetCategoriesQuery, Result<PageOf<Category>>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<PageOf<Category>>> HandleAsync(
    GetCategoriesQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      IQueryable<string> categoryNames = _context.Purchases.AsNoTracking()
        .OrderByMany(Order.DescendingBy<DbPurchase>(p => p.PurchaseId))
        .Select(p => p.Category)
        .Distinct();

      int totalCount = await categoryNames.CountAsync(cancellationToken);
      List<Category> categoryList = await categoryNames
        .Filter(CategoryContains(query.Search))
        .GetPage(query.PageNumber, query.PageSize)
        .Select(name => new Category(name))
        .ToListAsync(cancellationToken);

      PageOf<Category> page = Page.Of(categoryList, totalCount, query.PageSize, query.PageNumber);
      return Success.From(page);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<Category>>(Failure.Fatal(ex));
    }
  }

  private static Expression<Func<string, bool>> CategoryContains(string search)
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
    return categoryName => categoryName.ToUpper().Contains(search);
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }
}
