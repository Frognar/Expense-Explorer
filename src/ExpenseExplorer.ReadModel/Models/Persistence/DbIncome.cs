namespace ExpenseExplorer.ReadModel.Models.Persistence;

public sealed class DbIncome(
  string id,
  string source,
  decimal amount,
  string category,
  DateOnly receivedDate,
  string description)
{
  public string Id { get; init; } = id;

  public string Source { get; set; } = source;

  public decimal Amount { get; set; } = amount;

  public string Category { get; set; } = category;

  public DateOnly ReceivedDate { get; set; } = receivedDate;

  public string Description { get; set; } = description;
}
