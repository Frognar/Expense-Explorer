namespace ExpenseExplorer.API.Contract;

public record AddIncomeRequest(
  string Source,
  decimal Amount,
  string Category,
  DateOnly ReceivedDate,
  string? Description);
