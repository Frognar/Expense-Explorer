using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

/// <summary>
/// Aggregate root: a shopping receipt with its lines. All changes go through its methods,
/// so a receipt is always in a valid state.
/// </summary>
public sealed class Receipt
{
    private readonly List<ReceiptItem> _items;

    private Receipt(ReceiptId id, StoreName store, PurchaseDate purchaseDate, List<ReceiptItem> items)
    {
        Id = id;
        Store = store;
        PurchaseDate = purchaseDate;
        _items = items;
    }

    public ReceiptId Id { get; }

    public StoreName Store { get; private set; }

    public PurchaseDate PurchaseDate { get; private set; }

    public IReadOnlyList<ReceiptItem> Items => _items.AsReadOnly();

    public Money Total => Money.Sum(_items.Select(item => item.Total));

    public static Receipt Create(ReceiptId id, StoreName store, PurchaseDate purchaseDate)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(purchaseDate);

        return new Receipt(id, store, purchaseDate, []);
    }

    public void ChangeStore(StoreName store)
    {
        ArgumentNullException.ThrowIfNull(store);
        Store = store;
    }

    public void ChangePurchaseDate(PurchaseDate purchaseDate)
    {
        ArgumentNullException.ThrowIfNull(purchaseDate);
        PurchaseDate = purchaseDate;
    }

    public Result<ReceiptItem> AddItem(ReceiptItemId id, Purchase purchase)
    {
        if (FindItem(id) is not null)
        {
            return Result.Failure<ReceiptItem>(ReceiptErrors.ItemAlreadyExists);
        }

        ReceiptItem item = new(id, purchase);
        _items.Add(item);
        return Result.Success(item);
    }

    public Result<ReceiptItem> ChangeItem(ReceiptItemId id, Purchase purchase) =>
        ExistingItem(id).Map(item =>
        {
            item.Replace(purchase);
            return item;
        });

    public Result<ReceiptItem> RemoveItem(ReceiptItemId id) =>
        ExistingItem(id).Map(item =>
        {
            _items.Remove(item);
            return item;
        });

    /// <summary>Copies the store and all lines to a new receipt with new identifiers and the given date.</summary>
    public Receipt Duplicate(ReceiptId newId, PurchaseDate purchaseDate, Func<ReceiptItemId> newItemId)
    {
        ArgumentNullException.ThrowIfNull(newId);
        ArgumentNullException.ThrowIfNull(purchaseDate);
        ArgumentNullException.ThrowIfNull(newItemId);

        List<ReceiptItem> items = [.. _items.Select(item => new ReceiptItem(newItemId(), item.Purchase))];
        return new Receipt(newId, Store, purchaseDate, items);
    }

    private ReceiptItem? FindItem(ReceiptItemId id) => _items.Find(item => item.Id == id);

    private Result<ReceiptItem> ExistingItem(ReceiptItemId id) =>
        FindItem(id) is { } item
            ? Result.Success(item)
            : Result.Failure<ReceiptItem>(ReceiptErrors.ItemNotFound);
}
