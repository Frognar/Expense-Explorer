using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Receipts;

// Commands carry only domain types, so a command that exists is already valid.

public sealed record CreateReceipt(StoreName Store, PurchaseDate PurchaseDate);

/// <summary>A <c>null</c> field means "leave unchanged".</summary>
public sealed record ChangeReceipt(ReceiptId ReceiptId, StoreName? Store, PurchaseDate? PurchaseDate);

public sealed record DeleteReceipt(ReceiptId ReceiptId);

public sealed record DuplicateReceipt(ReceiptId ReceiptId, PurchaseDate PurchaseDate);

public sealed record AddReceiptItem(ReceiptId ReceiptId, Purchase Purchase);

public sealed record ChangeReceiptItem(ReceiptId ReceiptId, ReceiptItemId ItemId, Purchase Purchase);

public sealed record RemoveReceiptItem(ReceiptId ReceiptId, ReceiptItemId ItemId);

/// <summary>A whole receipt at once, e.g. read from a file a store provides.</summary>
public sealed record ImportReceipt(StoreName Store, PurchaseDate PurchaseDate, IReadOnlyList<Purchase> Purchases);
