using DotMaybe;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Domain.Receipts;

public sealed record ReceiptItem(
    ReceiptItemId Id,
    ReceiptId ReceiptId,
    Item Item,
    Category Category,
    Quantity Quantity,
    Money UnitPrice,
    Maybe<Money> Discount,
    Maybe<Description> Description);

public static class ReceiptItemFactory
{
    public static ReceiptItem Create(
        ReceiptItemId id,
        ReceiptId receiptId,
        Item item,
        Category category,
        Quantity quantity,
        Money unitPrice,
        Maybe<Money> discount,
        Maybe<Description> description)
        => new(id, receiptId, item, category, quantity, unitPrice, discount, description);
}