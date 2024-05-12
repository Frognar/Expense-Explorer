namespace ExpenseExplorer.ReadModel.Queries;

using System.Data.Common;
using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Failures;
using FunctionalCore.Monads;
using Microsoft.EntityFrameworkCore;

public sealed class GenerateReportQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GenerateReportQuery, Result<Report>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<Report>> HandleAsync(
    GenerateReportQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
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
      Report report = new(total, reportEntries);
      return Success.From(report);
    }
    catch (DbException ex)
    {
      return Fail.OfType<Report>(Failure.Fatal(ex));
    }
  }
}
