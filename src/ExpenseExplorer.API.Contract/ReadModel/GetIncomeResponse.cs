namespace ExpenseExplorer.API.Contract.ReadModel;

public record GetIncomeResponse(
  string Id,
  string Source,
  decimal Amount,
  string Category,
  DateOnly ReceivedDate,
  string Description);
