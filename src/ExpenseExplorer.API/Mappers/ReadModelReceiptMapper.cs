using ExpenseExplorer.API.Contract.ReadModel;
using ExpenseExplorer.ReadModel.Models;

namespace ExpenseExplorer.API.Mappers;

public static class ReadModelReceiptMapper
{
  public static GetStoresResponse MapToResponse(this PageOf<Store> page)
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

  public static GetItemsResponse MapToResponse(this PageOf<Item> page)
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

  public static GetCategoriesResponse MapToResponse(this PageOf<Category> page)
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

  public static GetSourcesResponse MapToResponse(this PageOf<Source> page)
  {
    ArgumentNullException.ThrowIfNull(page);
    return new GetSourcesResponse(
      page.Items.Select(source => source.Name),
      page.TotalCount,
      page.FilteredCount,
      page.PageSize,
      page.PageNumber,
      page.PageCount);
  }

  public static GetReceiptsResponse MapToResponse(this PageOf<ReceiptHeaders> page)
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

  public static GetReceiptResponse MapToResponse(this Receipt receipt)
  {
    ArgumentNullException.ThrowIfNull(receipt);
    return new GetReceiptResponse(
      receipt.Id,
      receipt.Store,
      receipt.PurchaseDate,
      receipt.Total,
      receipt.Purchases.Select(MapToResponse));
  }

  private static GetReceiptPurchaseResponse MapToResponse(this Purchase purchase)
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
