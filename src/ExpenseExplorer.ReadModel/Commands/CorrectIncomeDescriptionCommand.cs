using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record CorrectIncomeDescriptionCommand(string IncomeId, string Description) : ICommand<Unit>;
