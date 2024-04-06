namespace ExpenseExplorer.Infrastructure.Receipts.Projections.Models;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by Entity Framework")]
internal sealed class DbReceiptHeader(string id, string store, DateOnly purchaseDate, decimal total)
{
  public string Id { get; init; } = id;

  public string Store { get; init; } = store;

  public DateOnly PurchaseDate { get; init; } = purchaseDate;

  public decimal Total { get; init; } = total;
}
