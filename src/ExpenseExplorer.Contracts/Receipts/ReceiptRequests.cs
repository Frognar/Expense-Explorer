namespace ExpenseExplorer.Contracts.Receipts;

// Request fields are nullable on purpose: a missing value reaches the API validation
// and is reported next to the other errors instead of failing JSON deserialization.

public sealed record CreateReceiptRequest(string? Store, DateOnly? PurchaseDate);

/// <summary>Fields left out stay unchanged.</summary>
public sealed record UpdateReceiptRequest(string? Store, DateOnly? PurchaseDate);

public sealed record DuplicateReceiptRequest(DateOnly? PurchaseDate);

/// <summary>
/// <paramref name="Amount"/> is the price of the whole quantity as printed on the receipt,
/// <paramref name="Discount"/> is taken off that amount.
/// </summary>
public sealed record ReceiptItemRequest(
    string? Item,
    string? Category,
    decimal? Quantity,
    decimal? Amount,
    decimal? Discount,
    string? Description);
