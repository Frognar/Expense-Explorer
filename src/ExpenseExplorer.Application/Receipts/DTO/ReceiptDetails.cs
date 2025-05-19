using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.DTO;

public sealed record ReceiptDetails(Store Store, PurchaseDate PurchaseDate, ImmutableArray<ReceiptItem> Items);