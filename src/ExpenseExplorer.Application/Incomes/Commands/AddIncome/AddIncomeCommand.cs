using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Domain.Incomes;

namespace ExpenseExplorer.Application.Incomes.Commands;

public sealed record AddIncomeCommand(
  string Source,
  decimal Amount,
  string Category,
  DateOnly ReceivedDate,
  string? Description,
  DateOnly Today) : ICommand<Result<Income>>;
