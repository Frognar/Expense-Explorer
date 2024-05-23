namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record CorrectIncomeDescriptionCommand(string IncomeId, string Description) : ICommand<Unit>;
