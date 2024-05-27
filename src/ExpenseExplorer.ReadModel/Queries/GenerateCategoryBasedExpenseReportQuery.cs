namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Monads;

public sealed record GenerateCategoryBasedExpenseReportQuery(DateOnly From, DateOnly To)
  : IQuery<Result<CategoryBasedExpenseReport>>;
