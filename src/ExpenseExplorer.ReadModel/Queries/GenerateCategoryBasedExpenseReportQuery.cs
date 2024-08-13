using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed record GenerateCategoryBasedExpenseReportQuery(DateOnly From, DateOnly To)
  : IQuery<Result<CategoryBasedExpenseReport>>;
