namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;

public sealed record GenerateIncomeToExpenseReportQuery(DateOnly From, DateOnly To)
  : IQuery<Result<IncomeToExportReport>>;
