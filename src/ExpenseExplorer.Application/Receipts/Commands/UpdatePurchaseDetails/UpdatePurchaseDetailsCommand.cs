namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Domain.Receipts;

public sealed record UpdatePurchaseDetailsCommand(
  string ReceiptId,
  string PurchaseId,
  string? Item,
  string? Category,
  decimal? Quantity,
  decimal? UnitPrice,
  decimal? TotalDiscount,
  string? Description) : ICommand<Result<Receipt>>;
