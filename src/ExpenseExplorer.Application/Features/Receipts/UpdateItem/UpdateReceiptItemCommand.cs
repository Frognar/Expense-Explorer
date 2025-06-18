using DotMaybe;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.UpdateItem;

public sealed record UpdateReceiptItemCommand(
    ReceiptItemId ReceiptItemId,
    ReceiptId ReceiptId,
    Item Item,
    Category Category,
    Quantity Quantity,
    Money UnitPrice,
    Maybe<Money> Discount,
    Maybe<Description> Description)
    : ICommand<UpdateReceiptItemResponse>;

public static class UpdateReceiptItemCommandFactory
{
    public static Func<ReceiptItemId, ReceiptId, Item, Category, Quantity, Money, Maybe<Money>, Maybe<Description>,
        UpdateReceiptItemCommand> Create =>
        (riId, rId, i, c, q, up, d, de) => new UpdateReceiptItemCommand(riId, rId, i, c, q, up, d, de);
}