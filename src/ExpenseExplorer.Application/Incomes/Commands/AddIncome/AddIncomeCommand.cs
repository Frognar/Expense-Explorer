namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Domain.Incomes;

public sealed record AddIncomeCommand(
  string Source,
  decimal Amount,
  string Category,
  DateOnly ReceivedDate,
  string? Description,
  DateOnly Today) : ICommand<Result<Income>>;
