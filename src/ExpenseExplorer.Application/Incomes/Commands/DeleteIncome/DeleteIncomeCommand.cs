using CommandHub.Commands;
using DotResult;
using FunctionalCore;

namespace ExpenseExplorer.Application.Incomes.Commands;

public sealed record DeleteIncomeCommand(string IncomeId) : ICommand<Result<Unit>>;
