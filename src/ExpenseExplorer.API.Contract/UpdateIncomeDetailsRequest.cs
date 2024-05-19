namespace ExpenseExplorer.API.Contract;

public record UpdateIncomeDetailsRequest(
  string? Source,
  decimal? Amount,
  string? Category,
  DateOnly? ReceivedDate,
  string? Description);
