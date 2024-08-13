using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record AddIncomeCommand(
  string IncomeId,
  string Source,
  decimal Amount,
  string Category,
  DateOnly ReceivedDate,
  string Description) : ICommand<Unit>;
