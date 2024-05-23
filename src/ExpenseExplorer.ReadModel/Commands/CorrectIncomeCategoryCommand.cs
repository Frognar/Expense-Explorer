namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record CorrectIncomeCategoryCommand(string IncomeId, string Category) : ICommand<Unit>;
