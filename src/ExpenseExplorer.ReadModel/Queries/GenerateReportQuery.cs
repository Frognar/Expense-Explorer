namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using FunctionalCore.Monads;

public sealed record GenerateReportQuery(DateOnly From, DateOnly To) : IQuery<Result<Report>>;
