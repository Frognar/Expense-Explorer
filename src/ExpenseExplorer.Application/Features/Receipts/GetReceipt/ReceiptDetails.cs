namespace ExpenseExplorer.Application.Features.Receipts.GetReceipt;

public sealed record ReceiptDetails(
    Guid Id,
    string Store,
    DateOnly PurchaseDate,
    IEnumerable<ReceiptItemDetails> Items);