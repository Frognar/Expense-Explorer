using System.Collections.Immutable;

namespace ExpenseExplorer.Application.Receipts.DTO;

public sealed record ReceiptDetails(string Store, DateOnly PurchaseDate, ImmutableArray<ReceiptItem> Items);