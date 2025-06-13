using DotMaybe;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.DTO;

public sealed record ReceiptItem(
    ReceiptItemId Id,
    Item Item,
    Category Category,
    Money UnitPrice,
    Quantity Quantity,
    Maybe<Money> Discount,
    Maybe<Description> Description);