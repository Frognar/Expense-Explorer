namespace ExpenseExplorer.API.Mappers;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Domain.Receipts;

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

  public static OpenNewReceiptResponse MapToResponse(this Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    return new OpenNewReceiptResponse(receipt.Id.Value, receipt.Store.Name, receipt.PurchaseDate.Date);
  }
}
