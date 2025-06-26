using System.Collections.Immutable;
using DotMaybe;
using DotResult;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Domain.Receipts;

public record Receipt(
    ReceiptId Id,
    Store Store,
    PurchaseDate PurchaseDate,
    ImmutableList<ReceiptItem> Items);

public sealed record DeletedReceipt(
    ReceiptId Id,
    Store Store,
    PurchaseDate PurchaseDate,
    ImmutableList<ReceiptItem> Items)
    : Receipt(Id, Store, PurchaseDate, Items);

public static class ReceiptFactory
{
    public static Receipt Create(ReceiptId id, Store store, PurchaseDate purchaseDate)
        => new(id, store, purchaseDate, []);
}

public static class ReceiptHeaderExtensions
{
    public static Result<Receipt> UpdateStore(this Receipt receipt, Store store)
    {
        if (receipt is DeletedReceipt)
        {
            return Failure.NotFound(message: "Receipt is deleted");
        }

        if (receipt.Store == store)
        {
            return receipt;
        }

        return receipt with { Store = store };
    }

    public static Result<Receipt> UpdatePurchaseDate(this Receipt receipt, PurchaseDate purchaseDate)
    {
        if (receipt is DeletedReceipt)
        {
            return Failure.NotFound(message: "Receipt is deleted");
        }

        if (receipt.PurchaseDate == purchaseDate)
        {
            return receipt;
        }

        return receipt with { PurchaseDate = purchaseDate };
    }

    public static Result<Receipt> Delete(this Receipt receipt)
    {
        if (receipt is DeletedReceipt)
        {
            return Failure.NotFound(message: "Receipt is deleted");
        }

        return new DeletedReceipt(receipt.Id, receipt.Store, receipt.PurchaseDate, receipt.Items);
    }

    public static Result<Receipt> Duplicate(this Receipt receipt, PurchaseDate newPurchaseDate)
    {
        if (receipt is DeletedReceipt)
        {
            return Failure.NotFound(message: "Receipt is deleted");
        }

        ReceiptId newId = ReceiptId.Unique();
        return receipt with
        {
            Id = newId,
            PurchaseDate = newPurchaseDate,
            Items = receipt.Items.Select(item => item with
            {
                Id = ReceiptItemId.Unique(),
                ReceiptId = newId
            }).ToImmutableList()
        };
    }
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
        if (receipt is DeletedReceipt)
        {
            return Failure.NotFound(message: "Receipt is deleted");
        }

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

    public static Result<Receipt> UpdateItem(this Receipt receipt, ReceiptItem item)
    {
        if (receipt is DeletedReceipt)
        {
            return Failure.NotFound(message: "Receipt is deleted");
        }

        int index = receipt.Items.FindIndex(ri => ri.Id == item.Id);
        if (index == -1)
        {
            return Failure.NotFound(message: "Receipt item to update not found");
        }

        return receipt with
        {
            Items = receipt.Items.SetItem(index, item)
        };
    }

    public static Result<Receipt> RemoveItem(this Receipt receipt, ReceiptItemId id)
    {
        if (receipt is DeletedReceipt)
        {
            return Failure.NotFound(message: "Receipt is deleted");
        }

        if (receipt.Items.All(ri => ri.Id != id))
        {
            return Failure.NotFound(message: "Receipt item not found");
        }

        return receipt with { Items = receipt.Items.RemoveAll(ri => ri.Id == id) };
    }
}