namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record AddIncomeCommand(
  string IncomeId,
  string Source,
  decimal Amount,
  string Category,
  DateTime ReceivedDate,
  string Description) : ICommand<Unit>;
