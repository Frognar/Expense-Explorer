namespace ExpenseExplorer.Application.Dictionaries;

/// <summary>Distinct names already used on receipts, for autocomplete and filters.</summary>
public interface IDictionaryQueries
{
    Task<IReadOnlyList<string>> StoresAsync(DictionaryQuery query, CancellationToken cancellationToken);

    Task<IReadOnlyList<string>> ItemsAsync(DictionaryQuery query, CancellationToken cancellationToken);

    Task<IReadOnlyList<string>> CategoriesAsync(DictionaryQuery query, CancellationToken cancellationToken);
}

/// <summary>Case-insensitive "contains" search; an empty search returns the first <see cref="Limit"/> names.</summary>
public sealed record DictionaryQuery(string Search, int Limit)
{
    public const int DefaultLimit = 20;

    public const int MaxLimit = 1000;
}
