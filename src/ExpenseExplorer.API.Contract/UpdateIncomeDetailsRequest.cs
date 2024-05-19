namespace ExpenseExplorer.API.Contract;

public record UpdateIncomeDetailsRequest(
  string IncomeId,
  string? Source,
  decimal? Amount,
  string? Category,
  DateOnly? ReceivedDate,
  string? Description);
