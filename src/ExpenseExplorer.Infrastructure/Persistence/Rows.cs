namespace ExpenseExplorer.Infrastructure.Persistence;

// Storage shape of a receipt. Only the repository and read queries touch these;
// everything else works with the domain model.

internal sealed class ReceiptRow
{
    public Guid Id { get; set; }

    public string Store { get; set; } = "";

    public DateOnly PurchaseDate { get; set; }

    public List<ReceiptItemRow> Items { get; } = [];
}

internal sealed class ReceiptItemRow
{
    public Guid Id { get; set; }

    public Guid ReceiptId { get; set; }

    public int Position { get; set; }

    public string Item { get; set; } = "";

    public string Category { get; set; } = "";

    public decimal Quantity { get; set; }

    public decimal Amount { get; set; }

    public decimal Discount { get; set; }

    public string? Description { get; set; }
}

/// <summary>An old name kept after a rename, so imports that still read it get the new one.</summary>
internal sealed class NameAliasRow
{
    /// <summary>"store", "item" or "category".</summary>
    public string Kind { get; set; } = "";

    /// <summary>The old name in upper case, so letter case does not matter.</summary>
    public string Alias { get; set; } = "";

    public string Name { get; set; } = "";
}
