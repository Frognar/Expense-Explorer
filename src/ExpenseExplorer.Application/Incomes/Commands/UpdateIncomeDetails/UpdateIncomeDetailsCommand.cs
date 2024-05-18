namespace ExpenseExplorer.Application.Incomes.Commands;

public sealed record UpdateIncomeDetailsCommand(
  string IncomeId,
  string? Source,
  decimal? Amount,
  string? Category,
  DateOnly? ReceivedDate,
  string? Description,
  DateOnly Today);
