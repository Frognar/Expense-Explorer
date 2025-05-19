using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.DTO;

public sealed record ReceiptSummary(Store Store, PurchaseDate PurchaseDate, Money Total);