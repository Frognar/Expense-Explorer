using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Receipts;

public static class ReceiptResponses
{
    public static ReceiptResponse From(Receipt receipt) =>
        new(
            receipt.Id.Value,
            receipt.Store.Value,
            receipt.PurchaseDate.Value,
            receipt.Total.Value,
            receipt.Discount.Value,
            [.. receipt.Items.Select(From)]);

    public static ReceiptItemResponse From(ReceiptItem item) =>
        new(
            item.Id.Value,
            item.Purchase.Item.Value,
            item.Purchase.Category.Value,
            item.Purchase.Price.Quantity.Value,
            item.Purchase.Price.Amount.Value,
            item.Purchase.Price.Discount.Value,
            item.Total.Value,
            item.Purchase.Price.UnitPrice,
            item.Purchase.Description?.Value);
}
