namespace ExpenseExplorer.Contracts.Receipts;

// Request fields are nullable on purpose: a missing value reaches the API validation
// and is reported next to the other errors instead of failing JSON deserialization.

public sealed record CreateReceiptRequest(string? Store, DateOnly? PurchaseDate);

/// <summary>Fields left out stay unchanged.</summary>
public sealed record UpdateReceiptRequest(string? Store, DateOnly? PurchaseDate);

public sealed record DuplicateReceiptRequest(DateOnly? PurchaseDate);

public sealed record ReceiptItemRequest(
    string? Item,
    string? Category,
    decimal? Quantity,
    decimal? UnitPrice,
    decimal? Discount,
    string? Description);
