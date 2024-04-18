namespace ExpenseExplorer.ReadModel.Models.Persistence;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by Entity Framework")]
public sealed class DbReceipt(string id, string store, DateOnly purchaseDate, decimal total)
{
  [MaxLength(64)]
  public string Id { get; init; } = id;

  [MaxLength(128)]
  public string Store { get; init; } = store;

  public DateOnly PurchaseDate { get; init; } = purchaseDate;

  public decimal Total { get; set; } = total;
}
