namespace ExpenseExplorer.API.Contract;

public record UpdateIncomeDetailsResponse(
  string Id,
  string Source,
  decimal Amount,
  string Category,
  DateOnly ReceivedDate,
  string Description,
  ulong Version);
