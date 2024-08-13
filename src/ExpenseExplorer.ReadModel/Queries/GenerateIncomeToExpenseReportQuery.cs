using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed record GenerateIncomeToExpenseReportQuery(DateOnly From, DateOnly To)
  : IQuery<Result<IncomeToExportReport>>;
