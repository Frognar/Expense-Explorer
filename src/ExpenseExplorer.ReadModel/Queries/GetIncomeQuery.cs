using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed record GetIncomeQuery(string IncomeId) : IQuery<Result<Income>>;
