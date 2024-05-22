namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record CorrectIncomeAmountCommand(string IncomeId, decimal Amount) : ICommand<Unit>;
