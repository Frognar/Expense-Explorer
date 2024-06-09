namespace ExpenseExplorer.API.Mappers;

using ExpenseExplorer.API.Contract.ReadModel;

public static class ReadModelReceiptMapper
{
  public static GetStoresResponse MapToResponse(this ReadModel.Models.PageOf<ReadModel.Models.Store> page)
  {
    ArgumentNullException.ThrowIfNull(page);
    return new GetStoresResponse(
      page.Items.Select(store => store.Name),
      page.TotalCount,
      page.FilteredCount,
      page.PageSize,
      page.PageNumber,
      page.PageCount);
  }

  public static GetItemsResponse MapToResponse(this ReadModel.Models.PageOf<ReadModel.Models.Item> page)
  {
    ArgumentNullException.ThrowIfNull(page);
    return new GetItemsResponse(
      page.Items.Select(item => item.Name),
      page.TotalCount,
      page.FilteredCount,
      page.PageSize,
      page.PageNumber,
      page.PageCount);
  }

  public static GetCategoriesResponse MapToResponse(this ReadModel.Models.PageOf<ReadModel.Models.Category> page)
  {
    ArgumentNullException.ThrowIfNull(page);
    return new GetCategoriesResponse(
      page.Items.Select(category => category.Name),
      page.TotalCount,
      page.FilteredCount,
      page.PageSize,
      page.PageNumber,
      page.PageCount);
  }

  public static GetSourcesResponse MapToResponse(this ReadModel.Models.PageOf<ReadModel.Models.Source> page)
  {
    ArgumentNullException.ThrowIfNull(page);
    return new GetSourcesResponse(
      page.Items.Select(source => source.Name),
      page.TotalCount,
      page.PageSize,
      page.PageNumber,
      page.PageCount);
  }

  public static GetReceiptsResponse MapToResponse(this ReadModel.Models.PageOf<ReadModel.Models.ReceiptHeaders> page)
  {
    ArgumentNullException.ThrowIfNull(page);
    return new GetReceiptsResponse(
      page.Items.Select(r => new ReceiptHeaderResponse(r.Id, r.Store, r.PurchaseDate, r.Total)),
      page.TotalCount,
      page.FilteredCount,
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
