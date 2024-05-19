namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using FunctionalCore;
using FunctionalCore.Monads;

public sealed record DeleteIncomeCommand(string IncomeId) : ICommand<Result<Unit>>;
