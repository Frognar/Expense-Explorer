using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record DeleteIncomeCommand(string IncomeId) : ICommand<Unit>;
