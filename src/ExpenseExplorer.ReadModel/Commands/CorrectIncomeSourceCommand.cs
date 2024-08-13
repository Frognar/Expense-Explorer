using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record CorrectIncomeSourceCommand(string IncomeId, string Source) : ICommand<Unit>;
