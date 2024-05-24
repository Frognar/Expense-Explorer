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
      List<Income> incomes = await _context.Incomes.AsNoTracking()
        .Filter(
          ReceivedBetween(query.ReceivedAfter, query.ReceivedBefore),
          AmountBetween(query.MinAmount, query.MaxAmount),
          Search(query.Search))
        .OrderByMany(
          Order.AscendingBy<DbIncome>(i => i.ReceivedDate),
          Order.DescendingBy<DbIncome>(r => r.Id))
        .GetPage(query.PageNumber, query.PageSize)
        .Select(i => new Income(i.Id, i.Source, i.Amount, i.Category, i.ReceivedDate, i.Description))
        .ToListAsync(cancellationToken);

      int totalCount = await _context.Incomes.CountAsync(cancellationToken);
      PageOf<Income> response = Page.Of(incomes, totalCount, query.PageSize, query.PageNumber);
      return Success.From(response);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<Income>>(Failure.Fatal(ex));
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

  private static Expression<Func<DbIncome, bool>> Search(string search)
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
    return i => i.Source.ToUpper().Contains(search)
                || i.Category.ToUpper().Contains(search)
                || i.Description.ToUpper().Contains(search);
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }
}
