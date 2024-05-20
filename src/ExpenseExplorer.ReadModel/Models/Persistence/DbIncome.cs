namespace ExpenseExplorer.ReadModel.Models.Persistence;

public sealed record DbIncome(
  string Id,
  string Source,
  decimal Amount,
  string Category,
  DateOnly ReceivedDate,
  string Description);
