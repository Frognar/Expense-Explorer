using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.ReadModel.Models.Persistence;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by Entity Framework")]
[PrimaryKey(nameof(ReceiptId), nameof(PurchaseId))]
[Index(nameof(ReceiptId))]
public sealed class DbPurchase(
  string receiptId,
  string purchaseId,
  string item,
  string category,
  decimal quantity,
  decimal unitPrice,
  decimal totalDiscount,
  string description)
{
  [MaxLength(64)]
  public string ReceiptId { get; set; } = receiptId;

  [MaxLength(64)]
  public string PurchaseId { get; set; } = purchaseId;

  [MaxLength(128)]
  public string Item { get; set; } = item;

  [MaxLength(128)]
  public string Category { get; set; } = category;

  public decimal Quantity { get; set; } = quantity;

  public decimal UnitPrice { get; set; } = unitPrice;

  public decimal TotalDiscount { get; set; } = totalDiscount;

  [MaxLength(255)]
  public string Description { get; set; } = description;
}
