namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using DotResult;
using FunctionalCore;

public sealed record DeleteIncomeCommand(string IncomeId) : ICommand<Result<Unit>>;
