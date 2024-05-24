namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record DeleteIncomeCommand(string IncomeId) : ICommand<Unit>;
