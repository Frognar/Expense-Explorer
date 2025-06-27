using DotMaybe;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.AddItem;

internal sealed record AddReceiptItemCommand(
    ReceiptItemId Id,
    ReceiptId ReceiptId,
    Item Item,
    Category Category,
    Quantity Quantity,
    Money UnitPrice,
    Maybe<Money> Discount,
    Maybe<Description> Description)
    : ICommand<AddReceiptItemResponse>;

internal static class AddReceiptItemCommandFactory
{
    public static Func<ReceiptId, Item, Category, Quantity, Money, Maybe<Money>, Maybe<Description>, AddReceiptItemCommand> Create =>
        (rId, i, c, q, up, d, de) => new AddReceiptItemCommand(ReceiptItemId.Unique(), rId, i, c, q, up, d, de);
}