namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Monads;

public sealed record GenerateIncomeToExpenseReportQuery(DateOnly From, DateOnly To)
  : IQuery<Result<IncomeToExportReport>>;
