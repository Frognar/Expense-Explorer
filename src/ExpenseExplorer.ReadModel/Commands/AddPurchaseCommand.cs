namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record AddPurchaseCommand(
  string ReceiptId,
  string PurchaseId,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal TotalDiscount,
  string Description) : ICommand<Unit>;
