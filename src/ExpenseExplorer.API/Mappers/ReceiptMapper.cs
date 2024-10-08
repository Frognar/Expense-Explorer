using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.API.Mappers;

public static class ReceiptMapper
{
  public static OpenNewReceiptCommand MapToCommand(this OpenNewReceiptRequest request, DateOnly today)
  {
    ArgumentNullException.ThrowIfNull(request);
    return new OpenNewReceiptCommand(request.StoreName, request.PurchaseDate, today);
  }

  public static UpdateReceiptCommand MapToCommand(this UpdateReceiptRequest request, string receiptId, DateOnly today)
  {
    ArgumentNullException.ThrowIfNull(request);
    return new UpdateReceiptCommand(receiptId, request.StoreName, request.PurchaseDate, today);
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

  public static UpdatePurchaseDetailsCommand MapToCommand(
    this UpdatePurchaseRequest request,
    string receiptId,
    string purchaseId)
  {
    ArgumentNullException.ThrowIfNull(request);
    return new UpdatePurchaseDetailsCommand(
      receiptId,
      purchaseId,
      request.Item,
      request.Category,
      request.Quantity,
      request.UnitPrice,
      request.TotalDiscount,
      request.Description);
  }

  public static TResult MapTo<TResult>(this Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    if (typeof(TResult) == typeof(UpdatePurchaseResponse))
    {
      return (TResult)(object)new UpdatePurchaseResponse(
        receipt.Id.Value,
        receipt.Store.Name,
        receipt.PurchaseDate.Date,
        receipt.Purchases.Select(MapToResponse),
        receipt.Version.Value);
    }

    if (typeof(TResult) == typeof(AddPurchaseResponse))
    {
      return (TResult)(object)new AddPurchaseResponse(
        receipt.Id.Value,
        receipt.Store.Name,
        receipt.PurchaseDate.Date,
        receipt.Purchases.Select(MapToResponse),
        receipt.Version.Value);
    }

    if (typeof(TResult) == typeof(UpdateReceiptResponse))
    {
      return (TResult)(object)new UpdateReceiptResponse(
        receipt.Id.Value,
        receipt.Store.Name,
        receipt.PurchaseDate.Date,
        receipt.Version.Value);
    }

    return (TResult)(object)new OpenNewReceiptResponse(
      receipt.Id.Value,
      receipt.Store.Name,
      receipt.PurchaseDate.Date,
      receipt.Version.Value);
  }

  private static PurchaseResponse MapToResponse(this Purchase purchase)
  {
    ArgumentNullException.ThrowIfNull(purchase);
    return new PurchaseResponse(
      purchase.Id.Value,
      purchase.Item.Name,
      purchase.Category.Name,
      purchase.Quantity.Value,
      purchase.UnitPrice.Value,
      purchase.TotalDiscount.Value,
      purchase.Description.Value);
  }
}
