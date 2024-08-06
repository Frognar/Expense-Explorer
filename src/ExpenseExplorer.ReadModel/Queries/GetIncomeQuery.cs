namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;

public sealed record GetIncomeQuery(string IncomeId) : IQuery<Result<Income>>;
