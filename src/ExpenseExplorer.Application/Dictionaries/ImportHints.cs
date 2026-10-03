using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Dictionaries;

/// <summary>
/// Fills an imported receipt in the way earlier receipts were: renamed names become their new names,
/// and an item bought before gets the category it had last time. The category the import chose
/// stays only for items never seen before.
/// </summary>
public static class ImportHints
{
    public static async Task<ImportReceipt> ApplyAsync(
        INameHistory history,
        ImportReceipt receipt,
        CancellationToken cancellationToken)
    {
        NameAliases aliases = await history.AliasesAsync(cancellationToken);
        string[] items = [.. receipt.Purchases.Select(purchase => aliases.Resolve(NameKind.Item, purchase.Item.Value)).Distinct()];
        IReadOnlyDictionary<string, string> lastCategories = await history.LastCategoriesAsync(items, cancellationToken);
        return Apply(receipt, aliases, lastCategories);
    }

    public static ImportReceipt Apply(
        ImportReceipt receipt,
        NameAliases aliases,
        IReadOnlyDictionary<string, string> lastCategories) =>
        receipt with
        {
            Store = Renamed(receipt.Store, aliases.Resolve(NameKind.Store, receipt.Store.Value), StoreName.Create),
            Purchases = [.. receipt.Purchases.Select(purchase => Apply(purchase, aliases, lastCategories))],
        };

    private static Purchase Apply(Purchase purchase, NameAliases aliases, IReadOnlyDictionary<string, string> lastCategories)
    {
        ItemName item = Renamed(purchase.Item, aliases.Resolve(NameKind.Item, purchase.Item.Value), ItemName.Create);
        string category = lastCategories.TryGetValue(item.Value, out string? last)
            ? last
            : aliases.Resolve(NameKind.Category, purchase.Category.Value);

        return new Purchase(item, Renamed(purchase.Category, category, CategoryName.Create), purchase.Price, purchase.Description);
    }

    /// <summary>Names from the database passed the same rules when saved; one that no longer does is ignored.</summary>
    private static T Renamed<T>(T current, string name, Func<string, Domain.Common.Result<T>> create) =>
        create(name).Match(renamed => renamed, _ => current);
}
