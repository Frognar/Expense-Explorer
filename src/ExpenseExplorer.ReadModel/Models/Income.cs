namespace ExpenseExplorer.ReadModel.Models;

public sealed record Income(
  string Id,
  string Source,
  decimal Amount,
  string Category,
  DateOnly ReceivedDate,
  string Description);
