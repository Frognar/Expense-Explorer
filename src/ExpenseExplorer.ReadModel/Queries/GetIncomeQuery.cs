namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Monads;

public sealed record GetIncomeQuery(string IncomeId) : IQuery<Result<Income>>;
