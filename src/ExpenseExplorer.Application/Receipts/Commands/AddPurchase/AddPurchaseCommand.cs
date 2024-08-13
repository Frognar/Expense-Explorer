using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed record AddPurchaseCommand(
  string ReceiptId,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal? TotalDiscount,
  string? Description) : ICommand<Result<Receipt>>;
