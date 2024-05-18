namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Domain.Incomes;
using FunctionalCore.Monads;

public sealed record UpdateIncomeDetailsCommand(
  string IncomeId,
  string? Source,
  decimal? Amount,
  string? Category,
  DateOnly? ReceivedDate,
  string? Description,
  DateOnly Today) : ICommand<Result<Income>>;
