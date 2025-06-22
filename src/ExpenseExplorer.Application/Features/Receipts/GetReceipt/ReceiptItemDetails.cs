using DotMaybe;

namespace ExpenseExplorer.Application.Features.Receipts.GetReceipt;

public sealed record ReceiptItemDetails(
    Guid Id,
    string Item,
    string Category,
    decimal UnitPrice,
    decimal Quantity,
    Maybe<decimal> Discount,
    Maybe<string> Description);