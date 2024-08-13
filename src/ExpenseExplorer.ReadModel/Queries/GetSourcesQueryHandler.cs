using System.Data.Common;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed class GetSourcesQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetSourcesQuery, Result<PageOf<Source>>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<PageOf<Source>>> HandleAsync(
    GetSourcesQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      IQueryable<string> sourceQuery = _context.Incomes.AsNoTracking()
        .OrderByMany(
          Order.DescendingBy<DbIncome>(r => r.ReceivedDate),
          Order.DescendingBy<DbIncome>(r => r.Id))
        .Select(r => r.Source)
        .Distinct();

      int totalCount = await sourceQuery.CountAsync(cancellationToken);
      sourceQuery = sourceQuery.WhereContains(query.Search);
      int filteredCount = await sourceQuery.CountAsync(cancellationToken);
      List<Source> sourceList = await sourceQuery
        .GetPage(query.PageNumber, query.PageSize)
        .Select(name => new Source(name))
        .ToListAsync(cancellationToken);

      PageOf<Source> page = Page.Of(sourceList, totalCount, filteredCount, query.PageSize, query.PageNumber);
      return Success.From(page);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<Source>>(FailureFactory.Fatal(ex));
    }
  }
}
