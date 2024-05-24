namespace ExpenseExplorer.API.Contract.ReadModel;

public record IncomeResponse(
  string Id,
  string Source,
  decimal Amount,
  string Category,
  DateOnly ReceivedDate,
  string Description);
