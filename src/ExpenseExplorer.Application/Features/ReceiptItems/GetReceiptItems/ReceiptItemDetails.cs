using DotMaybe;

namespace ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

public sealed record ReceiptItemDetails(
    Guid Id,
    Guid ReceiptId,
    string Store,
    DateOnly PurchaseDate,
    string Item,
    string Category,
    decimal UnitPrice,
    decimal Quantity,
    Maybe<decimal> Discount,
    Maybe<string> Description);