using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record CorrectIncomeCategoryCommand(string IncomeId, string Category) : ICommand<Unit>;
