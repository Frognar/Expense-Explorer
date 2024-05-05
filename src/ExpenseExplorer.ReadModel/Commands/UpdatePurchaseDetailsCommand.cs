namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;

public record UpdatePurchaseDetailsCommand(
  string ReceiptId,
  string PurchaseId,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal TotalDiscount,
  string Description) : ICommand<Unit>;
