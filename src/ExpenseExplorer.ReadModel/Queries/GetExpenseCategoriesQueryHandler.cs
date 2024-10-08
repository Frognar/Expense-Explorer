using System.Data.Common;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed class GetExpenseCategoriesQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetExpenseCategoriesQuery, Result<PageOf<Category>>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<PageOf<Category>>> HandleAsync(
    GetExpenseCategoriesQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      IQueryable<string> categoryQuery = _context.Purchases.AsNoTracking()
        .OrderByMany(Order.DescendingBy<DbPurchase>(p => p.PurchaseId))
        .Select(p => p.Category)
        .Distinct();

      int totalCount = await categoryQuery.CountAsync(cancellationToken);
      categoryQuery = categoryQuery.WhereContains(query.Search);
      int filteredCount = await categoryQuery.CountAsync(cancellationToken);
      List<Category> categoryList = await categoryQuery
        .GetPage(query.PageNumber, query.PageSize)
        .Select(name => new Category(name))
        .ToListAsync(cancellationToken);

      PageOf<Category> page = Page.Of(categoryList, totalCount, filteredCount, query.PageSize, query.PageNumber);
      return Success.From(page);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<Category>>(FailureFactory.Fatal(ex));
    }
  }
}
