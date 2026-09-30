namespace ExpenseExplorer.Domain.Receipts;

/// <summary>What was bought on a receipt line. Every part is already valid, so any combination is valid.</summary>
public sealed record Purchase
{
    public Purchase(ItemName item, CategoryName category, LinePrice price, Description? description)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(price);

        Item = item;
        Category = category;
        Price = price;
        Description = description;
    }

    public ItemName Item { get; }

    public CategoryName Category { get; }

    public LinePrice Price { get; }

    public Description? Description { get; }
}
