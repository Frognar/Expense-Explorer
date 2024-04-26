namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Domain.Receipts;
using FunctionalCore.Monads;

public record AddPurchaseCommand(
  string ReceiptId,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal? TotalDiscount,
  string? Description) : ICommand<Result<Receipt>>;
