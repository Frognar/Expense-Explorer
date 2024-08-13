using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record CorrectIncomeAmountCommand(string IncomeId, decimal Amount) : ICommand<Unit>;
