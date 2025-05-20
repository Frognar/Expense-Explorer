using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.DTO;

public sealed record CreateReceiptRequest(Store Store, PurchaseDate PurchaseDate);