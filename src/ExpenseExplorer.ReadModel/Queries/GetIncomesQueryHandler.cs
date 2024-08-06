namespace ExpenseExplorer.ReadModel.Queries;

using System.Data.Common;
using System.Linq.Expressions;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

public sealed class GetIncomesQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetIncomesQuery, Result<PageOf<Income>>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<PageOf<Income>>> HandleAsync(
    GetIncomesQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      IQueryable<DbIncome> incomeQuery = _context.Incomes.AsNoTracking();
      int totalCount = await incomeQuery.CountAsync(cancellationToken);
      incomeQuery = incomeQuery
        .Filter(
          ReceivedBetween(query.ReceivedAfter, query.ReceivedBefore),
          AmountBetween(query.MinAmount, query.MaxAmount),
          SearchSource(query.Source),
          SearchCategory(query.Category),
          SearchDescription(query.Description));

      int filteredCount = await incomeQuery.CountAsync(cancellationToken);
      List<Income> incomes = await incomeQuery
        .OrderByMany(
          Order.By(GetSelector(query.SortBy), Order.GetDirection(query.SortOrder)),
          Order.AscendingBy<DbIncome>(i => i.Id))
        .GetPage(query.PageNumber, query.PageSize)
        .Select(i => new Income(i.Id, i.Source, i.Amount, i.Category, i.ReceivedDate, i.Description))
        .ToListAsync(cancellationToken);

      PageOf<Income> response = Page.Of(incomes, totalCount, filteredCount, query.PageSize, query.PageNumber);
      return Success.From(response);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<Income>>(FailureFactory.Fatal(ex));
    }
  }

  private static Expression<Func<DbIncome, bool>> ReceivedBetween(DateOnly min, DateOnly max)
  {
    return r => r.ReceivedDate >= min && r.ReceivedDate <= max;
  }

  private static Expression<Func<DbIncome, bool>> AmountBetween(decimal min, decimal max)
  {
    return r => r.Amount >= min && r.Amount <= max;
  }

  private static Expression<Func<DbIncome, bool>> SearchSource(string search)
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
    return i => i.Source.ToUpper().Contains(search);
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }

  private static Expression<Func<DbIncome, bool>> SearchCategory(string search)
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
    return i => i.Category.ToUpper().Contains(search);
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }

  private static Expression<Func<DbIncome, bool>> SearchDescription(string search)
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
    return i => i.Description.ToUpper().Contains(search);
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }

  private static Expression<Func<DbIncome, object>> GetSelector(string orderBy)
  {
    return orderBy.ToUpperInvariant() switch
    {
      "SOURCE" => income => income.Source,
      "AMOUNT" => income => income.Amount,
      "CATEGORY" => income => income.Category,
      "RECEIVEDDATE" => income => income.ReceivedDate,
      "DESCRIPTION" => income => income.Description,
      _ => income => income.ReceivedDate,
    };
  }
}
