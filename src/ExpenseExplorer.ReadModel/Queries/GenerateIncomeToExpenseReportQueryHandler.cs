using System.Data.Common;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed class GenerateIncomeToExpenseReportQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GenerateIncomeToExpenseReportQuery, Result<IncomeToExportReport>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<IncomeToExportReport>> HandleAsync(
    GenerateIncomeToExpenseReportQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      decimal totalIncome = await _context.Incomes.AsNoTracking()
        .Where(i => i.ReceivedDate >= query.From && i.ReceivedDate <= query.To)
        .SumAsync(i => i.Amount, cancellationToken);

      decimal totalExpense = await _context.Receipts.AsNoTracking()
        .Where(r => r.PurchaseDate >= query.From && r.PurchaseDate <= query.To)
        .SelectMany(r => r.Purchases)
        .SumAsync(p => (p.UnitPrice * p.Quantity) - p.TotalDiscount, cancellationToken);

      IncomeToExportReport report = new(query.From, query.To, totalIncome, totalExpense);
      return Success.From(report);
    }
    catch (DbException ex)
    {
      return Fail.OfType<IncomeToExportReport>(FailureFactory.Fatal(ex));
    }
  }
}
