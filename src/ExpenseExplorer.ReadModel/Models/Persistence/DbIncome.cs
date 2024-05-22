namespace ExpenseExplorer.ReadModel.Models.Persistence;

using System.ComponentModel.DataAnnotations;

public sealed class DbIncome(
  string id,
  string source,
  decimal amount,
  string category,
  DateOnly receivedDate,
  string description)
{
  [MaxLength(64)]
  public string Id { get; init; } = id;

  [MaxLength(128)]
  public string Source { get; set; } = source;

  public decimal Amount { get; set; } = amount;

  [MaxLength(128)]
  public string Category { get; set; } = category;

  public DateOnly ReceivedDate { get; set; } = receivedDate;

  [MaxLength(255)]
  public string Description { get; set; } = description;
}
