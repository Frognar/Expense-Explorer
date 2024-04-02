namespace ExpenseExplorer.API.Mappers;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public static class ReceiptMapper
{
  public static OpenNewReceiptCommand MapToCommand(this OpenNewReceiptRequest request, DateOnly today)
  {
    ArgumentNullException.ThrowIfNull(request);
    return new OpenNewReceiptCommand(request.StoreName, request.PurchaseDate, today);
  }

  public static AddPurchaseCommand MapToCommand(this AddPurchaseRequest request, string receiptId)
  {
    ArgumentNullException.ThrowIfNull(request);
    return new AddPurchaseCommand(
      receiptId,
      request.Item,
      request.Category,
      request.Quantity,
      request.UnitPrice,
      request.TotalDiscount,
      request.Description);
  }

  public static ReceiptResponse MapToResponse(this Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    return new ReceiptResponse(
      receipt.Id.Value,
      receipt.Store.Name,
      receipt.PurchaseDate.Date,
      receipt.Purchases.Select(MapToResponse));
  }

  private static PurchaseResponse MapToResponse(this Purchase purchase, int index)
  {
    ArgumentNullException.ThrowIfNull(purchase);
    return new PurchaseResponse(
      index + 1,
      purchase.Item.Name,
      purchase.Category.Name,
      purchase.Quantity.Value,
      purchase.UnitPrice.Value,
      purchase.TotalDiscount.Value,
      purchase.Description.Value);
  }
}
