namespace ExpenseExplorer.ReadModel.Queries;

using System.Linq.Expressions;
using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using FunctionalCore.Monads;
using Microsoft.EntityFrameworkCore;

public sealed class GetIncomeQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetIncomeQuery, Result<Income>>
{
  private static readonly Expression<Func<DbIncome, Income>> _incomeSelector = i =>
    new Income(
      i.Id,
      i.Source,
      i.Amount,
      i.Category,
      i.ReceivedDate,
      i.Description);

  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<Income>> HandleAsync(
    GetIncomeQuery query,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(query);
    Income? income = await _context.Incomes
      .Where(i => i.Id == query.IncomeId)
      .Select(_incomeSelector)
      .FirstOrDefaultAsync(cancellationToken: cancellationToken);

    return income is not null
      ? Success.From(income)
      : Fail.OfType<Income>(Failure.NotFound("Income not found.", query.IncomeId));
  }
}
