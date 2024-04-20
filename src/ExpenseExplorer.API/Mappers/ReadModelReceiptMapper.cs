namespace ExpenseExplorer.API.Mappers;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Contract.ReadModel;

public static class ReadModelReceiptMapper
{
  public static GetReceiptsResponse MapToResponse(this ReadModel.Models.PageOf<ReadModel.Models.ReceiptHeaders> page)
  {
    ArgumentNullException.ThrowIfNull(page);
    return new GetReceiptsResponse(
      page.Items.Select(r => new ReceiptHeaderResponse(r.Id, r.Store, r.PurchaseDate, r.Total)),
      page.TotalCount,
      page.PageSize,
      page.PageNumber,
      page.PageCount);
  }

  public static GetReceiptResponse MapToResponse(this ReadModel.Models.Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    return new GetReceiptResponse(
      receipt.Id,
      receipt.Store,
      receipt.PurchaseDate,
      receipt.Total,
      receipt.Purchases.Select(MapToResponse));
  }

  private static GetReceiptPurchaseResponse MapToResponse(this ReadModel.Models.Purchase purchase)
  {
    ArgumentNullException.ThrowIfNull(purchase);
    return new GetReceiptPurchaseResponse(
      purchase.Id,
      purchase.Item,
      purchase.Category,
      purchase.Quantity,
      purchase.UnitPrice,
      purchase.TotalDiscount,
      purchase.Description);
  }
}
