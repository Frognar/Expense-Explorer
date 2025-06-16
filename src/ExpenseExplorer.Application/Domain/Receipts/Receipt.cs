using System.Collections.Immutable;
using DotMaybe;
using DotResult;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Domain.Receipts;

public sealed record Receipt(ReceiptId Id, Store Store, PurchaseDate PurchaseDate, ImmutableList<ReceiptItem> Items);

public static class ReceiptFactory
{
    public static Receipt Create(ReceiptId id, Store store, PurchaseDate purchaseDate)
        => new(id, store, purchaseDate, []);
}

public static class ReceiptExtensions
{
    public static Result<Receipt> AddItem(
        this Receipt receipt,
        ReceiptItemId id,
        Item item,
        Category category,
        Quantity quantity,
        Money unitPrice,
        Maybe<Money> discount,
        Maybe<Description> description)
    {
        bool isDiscountExceeding = discount.Match(none: () => false, some: d => unitPrice * quantity < d);
        if (isDiscountExceeding)
        {
            return Failure.Validation(
                nameof(discount),
                "Discount cannot be greater than the item's total value before discount.");
        }

        return receipt with
        {
            Items = receipt.Items.Add(ReceiptItemFactory.Create(
                id, receipt.Id, item, category, quantity, unitPrice, discount, description))
        };
    }
}