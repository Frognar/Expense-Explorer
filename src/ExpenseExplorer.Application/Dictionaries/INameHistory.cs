namespace ExpenseExplorer.Application.Dictionaries;

/// <summary>What earlier receipts and dictionary changes say about names, for filling in imported receipts.</summary>
public interface INameHistory
{
    Task<NameAliases> AliasesAsync(CancellationToken cancellationToken);

    /// <summary>For each of <paramref name="items"/> bought before, the category of its most recent purchase.</summary>
    Task<IReadOnlyDictionary<string, string>> LastCategoriesAsync(
        IReadOnlyCollection<string> items,
        CancellationToken cancellationToken);
}
