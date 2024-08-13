using System.Data.Common;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed class GetIncomeCategoriesQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetIncomeCategoriesQuery, Result<PageOf<Category>>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<PageOf<Category>>> HandleAsync(
    GetIncomeCategoriesQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      IQueryable<string> categoryQuery = _context.Incomes.AsNoTracking()
        .OrderByMany(Order.DescendingBy<DbIncome>(p => p.Id))
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
