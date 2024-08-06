namespace ExpenseExplorer.ReadModel.Queries;

using System.Data.Common;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

public sealed class GenerateCategoryBasedExpenseReportQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GenerateCategoryBasedExpenseReportQuery, Result<CategoryBasedExpenseReport>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<CategoryBasedExpenseReport>> HandleAsync(
    GenerateCategoryBasedExpenseReportQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      Dictionary<string, decimal> data = await _context.Receipts.AsNoTracking()
        .Where(r => r.PurchaseDate >= query.From && r.PurchaseDate <= query.To)
        .SelectMany(r => r.Purchases)
        .Select(p => new { p.Category, TotalPrice = (p.Quantity * p.UnitPrice) - p.TotalDiscount })
        .GroupBy(d => d.Category)
        .Select(d => new { Category = d.Key, TotalCost = d.Sum(p => p.TotalPrice) })
        .OrderByDescending(d => d.TotalCost)
        .ThenBy(d => d.Category)
        .ToDictionaryAsync(d => d.Category, d => d.TotalCost, cancellationToken);

      decimal total = data.Values.Sum();
      List<ReportEntry> reportEntries = data.Select(d => new ReportEntry(d.Key, d.Value)).ToList();
      CategoryBasedExpenseReport report = new(query.From, query.To, total, reportEntries);
      return Success.From(report);
    }
    catch (DbException ex)
    {
      return Fail.OfType<CategoryBasedExpenseReport>(FailureFactory.Fatal(ex));
    }
  }
}
