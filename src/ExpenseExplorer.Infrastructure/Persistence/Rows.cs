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

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public string? Description { get; set; }
}
