namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record CorrectIncomeSourceCommand(string IncomeId, string Source) : ICommand<Unit>;
